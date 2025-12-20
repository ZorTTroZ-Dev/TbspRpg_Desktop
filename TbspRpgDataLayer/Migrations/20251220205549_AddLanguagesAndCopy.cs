using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbspRpgDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguagesAndCopy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureLanguage",
                columns: table => new
                {
                    AdventuresId = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguagesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureLanguage", x => new { x.AdventuresId, x.LanguagesId });
                    table.ForeignKey(
                        name: "FK_AdventureLanguage_Adventures_AdventuresId",
                        column: x => x.AdventuresId,
                        principalTable: "Adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdventureLanguage_Languages_LanguagesId",
                        column: x => x.LanguagesId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Copy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScriptId = table.Column<int>(type: "INTEGER", nullable: true),
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Copy_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Copy_Scripts_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdventureLanguage_LanguagesId",
                table: "AdventureLanguage",
                column: "LanguagesId");

            migrationBuilder.CreateIndex(
                name: "IX_Copy_LanguageId",
                table: "Copy",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Copy_ScriptId",
                table: "Copy",
                column: "ScriptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdventureLanguage");

            migrationBuilder.DropTable(
                name: "Copy");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
