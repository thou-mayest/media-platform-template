using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Infrastructure.Migrations
{
    /// <summary>
    /// Moves tags from inline (PostId, NormalizedName, Name) rows in PostTags into a dedicated Tags table,
    /// turning PostTags into a (PostId, TagId) join table. Existing tag data is preserved.
    /// </summary>
    public partial class SeparateTagEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. New Tags table.
            migrationBuilder.Sql("""
                CREATE TABLE "Posts"."Tags" (
                    "Id" uuid NOT NULL,
                    "Name" character varying(50) NOT NULL,
                    "NormalizedName" character varying(50) NOT NULL,
                    "UsageCount" integer NOT NULL DEFAULT 0,
                    "CreatedAt" timestamp with time zone NOT NULL,
                    "UpdateDate" timestamp with time zone NULL,
                    "Version" uuid NOT NULL,
                    CONSTRAINT "PK_Tags" PRIMARY KEY ("Id")
                );
                CREATE UNIQUE INDEX "IX_Tags_NormalizedName" ON "Posts"."Tags" ("NormalizedName");
                """);

            // 2. One Tags row per distinct normalized name. Display name = the earliest post's spelling.
            migrationBuilder.Sql("""
                INSERT INTO "Posts"."Tags" ("Id", "Name", "NormalizedName", "UsageCount", "CreatedAt", "Version")
                SELECT gen_random_uuid(), first."Name", grouped."NormalizedName", grouped.usage, grouped.created_at, gen_random_uuid()
                FROM (
                    SELECT pt."NormalizedName", COUNT(*)::int AS usage, MIN(p."CreatedDate") AS created_at
                    FROM "Posts"."PostTags" pt
                    JOIN "Posts"."Posts" p ON p."Id" = pt."PostId"
                    GROUP BY pt."NormalizedName"
                ) grouped
                CROSS JOIN LATERAL (
                    SELECT pt."Name"
                    FROM "Posts"."PostTags" pt
                    JOIN "Posts"."Posts" p ON p."Id" = pt."PostId"
                    WHERE pt."NormalizedName" = grouped."NormalizedName"
                    ORDER BY p."CreatedDate", pt."Name"
                    LIMIT 1
                ) first;
                """);

            // 3. Point PostTags at Tags and drop the inline columns.
            migrationBuilder.Sql("""
                ALTER TABLE "Posts"."PostTags" ADD COLUMN "TagId" uuid NULL;

                UPDATE "Posts"."PostTags" pt
                SET "TagId" = t."Id"
                FROM "Posts"."Tags" t
                WHERE t."NormalizedName" = pt."NormalizedName";

                ALTER TABLE "Posts"."PostTags" ALTER COLUMN "TagId" SET NOT NULL;
                ALTER TABLE "Posts"."PostTags" DROP CONSTRAINT "PK_PostTags";
                ALTER TABLE "Posts"."PostTags" DROP COLUMN "NormalizedName";
                ALTER TABLE "Posts"."PostTags" DROP COLUMN "Name";
                ALTER TABLE "Posts"."PostTags" ADD CONSTRAINT "PK_PostTags" PRIMARY KEY ("PostId", "TagId");
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId_PostId",
                schema: "Posts",
                table: "PostTags",
                columns: new[] { "TagId", "PostId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Tags_TagId",
                schema: "Posts",
                table: "PostTags",
                column: "TagId",
                principalSchema: "Posts",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Tags_TagId",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropIndex(
                name: "IX_PostTags_TagId_PostId",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.Sql("""
                ALTER TABLE "Posts"."PostTags" ADD COLUMN "NormalizedName" character varying(50) NULL;
                ALTER TABLE "Posts"."PostTags" ADD COLUMN "Name" character varying(50) NULL;

                UPDATE "Posts"."PostTags" pt
                SET "NormalizedName" = t."NormalizedName", "Name" = t."Name"
                FROM "Posts"."Tags" t
                WHERE t."Id" = pt."TagId";

                ALTER TABLE "Posts"."PostTags" ALTER COLUMN "NormalizedName" SET NOT NULL;
                ALTER TABLE "Posts"."PostTags" ALTER COLUMN "Name" SET NOT NULL;
                ALTER TABLE "Posts"."PostTags" DROP CONSTRAINT "PK_PostTags";
                ALTER TABLE "Posts"."PostTags" DROP COLUMN "TagId";
                ALTER TABLE "Posts"."PostTags" ADD CONSTRAINT "PK_PostTags" PRIMARY KEY ("PostId", "NormalizedName");

                DROP TABLE "Posts"."Tags";
                """);
        }
    }
}
