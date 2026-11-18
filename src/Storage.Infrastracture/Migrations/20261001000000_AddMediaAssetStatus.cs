using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Storage.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaAssetStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Storage",
                table: "MediaAssets",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Completed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Storage",
                table: "MediaAssets");
        }
    }
}
