using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTagToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Tags_TagId",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTags",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropIndex(
                name: "IX_PostTags_TagId",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Status_NormalizedCategory",
                schema: "Posts",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TagId",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "Posts",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "NormalizedCategory",
                schema: "Posts",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                schema: "Posts",
                table: "PostTags",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Posts",
                table: "PostTags",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "Posts",
                table: "Posts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTags",
                schema: "Posts",
                table: "PostTags",
                columns: new[] { "PostId", "NormalizedName" });

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Posts",
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Name", "NormalizedName", "UpdateDate", "Version" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Uncategorized", "uncategorized", null, new Guid("00000000-0000-0000-0000-000000000002") });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                schema: "Posts",
                table: "Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Status_CategoryId",
                schema: "Posts",
                table: "Posts",
                columns: new[] { "Status", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NormalizedName",
                schema: "Posts",
                table: "Categories",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                schema: "Posts",
                table: "Posts",
                column: "CategoryId",
                principalSchema: "Posts",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                schema: "Posts",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTags",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CategoryId",
                schema: "Posts",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Status_CategoryId",
                schema: "Posts",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Posts",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "Posts",
                table: "Posts");

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                schema: "Posts",
                table: "PostTags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "Posts",
                table: "Posts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCategory",
                schema: "Posts",
                table: "Posts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTags",
                schema: "Posts",
                table: "PostTags",
                columns: new[] { "PostId", "TagId" });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                schema: "Posts",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Status_NormalizedCategory",
                schema: "Posts",
                table: "Posts",
                columns: new[] { "Status", "NormalizedCategory" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NormalizedName",
                schema: "Posts",
                table: "Tags",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Tags_TagId",
                schema: "Posts",
                table: "PostTags",
                column: "TagId",
                principalSchema: "Posts",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
