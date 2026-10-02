using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Profiles");

            migrationBuilder.CreateTable(
                name: "ActorProfiles",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Profession = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Bio = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    AvatarStorageKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AlbumCount = table.Column<int>(type: "integer", nullable: false),
                    MediaCount = table.Column<int>(type: "integer", nullable: false),
                    FollowerCount = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    IsIndexable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<Guid>(type: "uuid", nullable: false),
                    SocialLinks = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorProfiles_Indexable_CreatedDate",
                schema: "Profiles",
                table: "ActorProfiles",
                columns: new[] { "CreatedDate", "Id" },
                filter: "\"IsIndexable\"");

            migrationBuilder.CreateIndex(
                name: "IX_ActorProfiles_Slug",
                schema: "Profiles",
                table: "ActorProfiles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActorProfiles_UserId",
                schema: "Profiles",
                table: "ActorProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorProfiles",
                schema: "Profiles");
        }
    }
}
