using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastracture.Migrations;

public partial class UpdateUserValueObjects : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            DECLARE
                has_password_hash boolean;
            BEGIN
                SELECT EXISTS (
                    SELECT 1 FROM information_schema.columns
                    WHERE table_schema = 'Users'
                      AND table_name = 'Users'
                      AND column_name = 'PasswordHash'
                ) INTO has_password_hash;

                ALTER TABLE "Users"."Users" DROP CONSTRAINT IF EXISTS "CK_Users_Role";

                IF has_password_hash THEN
                    ALTER TABLE "Users"."Users" RENAME COLUMN "PasswordHash" TO "Password";
                    ALTER TABLE "Users"."Users"
                    ALTER COLUMN "Role" TYPE character varying(100)
                    USING CASE "Role"
                        WHEN 1 THEN 'Admin'
                        WHEN 2 THEN 'User'
                        WHEN 3 THEN 'PremiumUser'
                        ELSE 'User'
                    END;
                ELSE
                    ALTER TABLE "Users"."Users"
                    ALTER COLUMN "Role" TYPE character varying(100)
                    USING CASE "Role"
                        WHEN 0 THEN 'Admin'
                        WHEN 1 THEN 'User'
                        WHEN 2 THEN 'PremiumUser'
                        ELSE 'User'
                    END;
                END IF;

                ALTER TABLE "Users"."Users"
                ALTER COLUMN "Email" TYPE character varying(256);
            END $$;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        throw new NotSupportedException("User value-object migration cannot be rolled back safely.");
}
