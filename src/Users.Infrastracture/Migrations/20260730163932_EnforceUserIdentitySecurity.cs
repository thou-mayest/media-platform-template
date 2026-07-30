using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class EnforceUserIdentitySecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Users\".\"Users\" SET \"Email\" = LOWER(BTRIM(\"Email\"));");
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
            migrationBuilder.Sql("UPDATE \"Users\".\"Users\" SET \"Password\" = '!PASSWORD-RESET-REQUIRED!';");

            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX IF NOT EXISTS "UX_Users_Email"
                ON "Users"."Users" ("Email");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "This migration invalidates legacy plaintext passwords and cannot be rolled back safely.");
        }
    }
}
