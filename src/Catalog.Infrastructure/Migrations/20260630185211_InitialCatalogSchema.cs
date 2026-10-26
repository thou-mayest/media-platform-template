using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalogSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "catalog_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "catalog_creators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Disciplines = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Accent = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Initials = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Since = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_creators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "catalog_tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "catalog_videos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    Slug = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Medium = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    Size = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    AspectRatio = table.Column<double>(type: "double precision", nullable: false),
                    Background = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_catalog_videos_catalog_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "catalog_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalog_videos_catalog_creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "catalog_creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "catalog_video_tags",
                columns: table => new
                {
                    VideoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_video_tags", x => new { x.VideoId, x.TagId });
                    table.ForeignKey(
                        name: "FK_catalog_video_tags_catalog_tags_TagId",
                        column: x => x.TagId,
                        principalTable: "catalog_tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_catalog_video_tags_catalog_videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "catalog_videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalog_categories_Name",
                table: "catalog_categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_categories_Slug",
                table: "catalog_categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_creators_Slug",
                table: "catalog_creators",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_tags_Name",
                table: "catalog_tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_tags_Slug",
                table: "catalog_tags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_video_tags_TagId",
                table: "catalog_video_tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_videos_CategoryId",
                table: "catalog_videos",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_videos_CreatorId",
                table: "catalog_videos",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_videos_DisplayOrder",
                table: "catalog_videos",
                column: "DisplayOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_videos_Slug",
                table: "catalog_videos",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_videos_Year_DisplayOrder",
                table: "catalog_videos",
                columns: new[] { "Year", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalog_video_tags");

            migrationBuilder.DropTable(
                name: "catalog_tags");

            migrationBuilder.DropTable(
                name: "catalog_videos");

            migrationBuilder.DropTable(
                name: "catalog_categories");

            migrationBuilder.DropTable(
                name: "catalog_creators");
        }
    }
}
