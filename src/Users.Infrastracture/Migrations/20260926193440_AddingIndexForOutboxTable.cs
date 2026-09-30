using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexForOutboxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOnUtc",
                schema: "Users",
                table: "OutboxMessages",
                column: "ProcessedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ProcessedOnUtc",
                schema: "Users",
                table: "OutboxMessages");
        }
    }
}
