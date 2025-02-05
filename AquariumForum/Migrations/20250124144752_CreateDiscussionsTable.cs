using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumForum.Migrations
{
    /// <inheritdoc />
    public partial class CreateDiscussionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discussions",
                columns: table => new
                {
                    DiscussionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageFilename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussions", x => x.DiscussionId);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscussionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_Discussions_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussions",
                        principalColumn: "DiscussionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_DiscussionId",
                table: "Comment",
                column: "DiscussionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Discussions");
        }
    }
}
