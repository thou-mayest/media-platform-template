using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Users.Infrastracture.Persistence;

#nullable disable

namespace Users.Infrastracture.Migrations;

[DbContext(typeof(UsersDbContext))]
[Migration("20260730140246_SecureUserCredentials")]
public sealed class SecureUserCredentials : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Compatibility marker for databases that applied this branch before it merged upstream.
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
