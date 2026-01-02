using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbspRpgDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ScriptOrderedIncludes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScriptScript");

            migrationBuilder.CreateTable(
                name: "ScriptIncludes",
                columns: table => new
                {
                    IncludedInId = table.Column<int>(type: "INTEGER", nullable: false),
                    IncludesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptIncludes", x => new { x.IncludedInId, x.IncludesId });
                    table.ForeignKey(
                        name: "FK_ScriptIncludes_Scripts_IncludedInId",
                        column: x => x.IncludedInId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScriptIncludes_Scripts_IncludesId",
                        column: x => x.IncludesId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScriptIncludes_IncludesId",
                table: "ScriptIncludes",
                column: "IncludesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScriptIncludes");

            migrationBuilder.CreateTable(
                name: "ScriptScript",
                columns: table => new
                {
                    IncludedInId = table.Column<int>(type: "INTEGER", nullable: false),
                    IncludesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptScript", x => new { x.IncludedInId, x.IncludesId });
                    table.ForeignKey(
                        name: "FK_ScriptScript_Scripts_IncludedInId",
                        column: x => x.IncludedInId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScriptScript_Scripts_IncludesId",
                        column: x => x.IncludesId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScriptScript_IncludesId",
                table: "ScriptScript",
                column: "IncludesId");
        }
    }
}
