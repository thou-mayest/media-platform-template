using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class SecureUserCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                schema: "Users",
                table: "Users",
                newName: "PasswordHash");

            // Existing credentials were plaintext and cannot be converted safely in SQL.
            // Normalize identities, fail on collisions, then require password reset/bootstrap recovery.
            migrationBuilder.Sql("UPDATE \"Users\".\"Users\" SET \"Email\" = UPPER(BTRIM(\"Email\"));");
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM "Users"."Users"
                        GROUP BY "Email"
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Duplicate normalized user emails must be resolved before migration.';
                    END IF;
                END $$;
                """);
            migrationBuilder.Sql("UPDATE \"Users\".\"Users\" SET \"PasswordHash\" = '!PASSWORD-RESET-REQUIRED!';");
            migrationBuilder.Sql("UPDATE \"Users\".\"Users\" SET \"Role\" = \"Role\" + 1;");

            migrationBuilder.CreateIndex(
                name: "UX_Users_Email",
                schema: "Users",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Role",
                schema: "Users",
                table: "Users",
                sql: "\"Role\" IN (1, 2, 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "This migration invalidates legacy plaintext passwords and cannot be rolled back safely.");
        }
    }
}
