using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueUserEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "Users"."Users"
                SET "Email" = LOWER(BTRIM("Email"))
                WHERE "Email" <> LOWER(BTRIM("Email"));
                """);
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM "Users"."Users"
                        GROUP BY "Email" HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Duplicate normalized user emails must be resolved before migration.';
                    END IF;
                END $$;
                """);

            migrationBuilder.CreateIndex(
                name: "UX_Users_Email",
                schema: "Users",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Users_Email",
                schema: "Users",
                table: "Users");
        }
    }
}
