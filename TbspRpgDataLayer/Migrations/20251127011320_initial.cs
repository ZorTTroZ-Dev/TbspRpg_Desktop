using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbspRpgDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdventureObjectGameStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdventureObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdventureObjectState = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureObjectGameStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureObjectLocation",
                columns: table => new
                {
                    AdventureObjectsId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureObjectLocation", x => new { x.AdventureObjectsId, x.LocationsId });
                });

            migrationBuilder.CreateTable(
                name: "AdventureObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    NameSourceKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    DescriptionSourceKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: false),
                    InitializationScriptId = table.Column<int>(type: "INTEGER", nullable: true),
                    ActionScriptId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adventures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    InitialSourceKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    DescriptionSourceKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    InitializationScriptId = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminationScriptId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adventures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scripts_Adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Initial = table.Column<bool>(type: "INTEGER", nullable: false),
                    Final = table.Column<bool>(type: "INTEGER", nullable: false),
                    SourceKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: false),
                    EnterScriptId = table.Column<int>(type: "INTEGER", nullable: true),
                    ExitScriptId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Locations_Scripts_EnterScriptId",
                        column: x => x.EnterScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Locations_Scripts_ExitScriptId",
                        column: x => x.ExitScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScriptScript",
                columns: table => new
                {
                    IncludedInId = table.Column<int>(type: "INTEGER", nullable: false),
                    IncludesId = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SourcesEn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScriptId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourcesEn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourcesEn_Scripts_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SourcesEsp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScriptId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourcesEsp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourcesEsp_Scripts_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    LocationUpdateTimeStamp = table.Column<long>(type: "INTEGER", nullable: false),
                    GameState = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    SourceKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    RouteTakenSourceKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationLocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    RouteTakenScriptId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routes_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routes_Scripts_RouteTakenScriptId",
                        column: x => x.RouteTakenScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SourceKey = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contents_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdventureObjectGameStates_AdventureObjectId",
                table: "AdventureObjectGameStates",
                column: "AdventureObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AdventureObjectGameStates_GameId",
                table: "AdventureObjectGameStates",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_AdventureObjectLocation_LocationsId",
                table: "AdventureObjectLocation",
                column: "LocationsId");

            migrationBuilder.CreateIndex(
                name: "IX_AdventureObjects_ActionScriptId",
                table: "AdventureObjects",
                column: "ActionScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_AdventureObjects_AdventureId",
                table: "AdventureObjects",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_AdventureObjects_InitializationScriptId",
                table: "AdventureObjects",
                column: "InitializationScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Adventures_InitializationScriptId",
                table: "Adventures",
                column: "InitializationScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Adventures_TerminationScriptId",
                table: "Adventures",
                column: "TerminationScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_GameId",
                table: "Contents",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AdventureId",
                table: "Games",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_LocationId",
                table: "Games",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_AdventureId",
                table: "Locations",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_EnterScriptId",
                table: "Locations",
                column: "EnterScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ExitScriptId",
                table: "Locations",
                column: "ExitScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationLocationId",
                table: "Routes",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_LocationId",
                table: "Routes",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteTakenScriptId",
                table: "Routes",
                column: "RouteTakenScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_AdventureId",
                table: "Scripts",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_Content",
                table: "Scripts",
                column: "Content");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptScript_IncludesId",
                table: "ScriptScript",
                column: "IncludesId");

            migrationBuilder.CreateIndex(
                name: "IX_SourcesEn_ScriptId",
                table: "SourcesEn",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_SourcesEsp_ScriptId",
                table: "SourcesEsp",
                column: "ScriptId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureObjectGameStates_AdventureObjects_AdventureObjectId",
                table: "AdventureObjectGameStates",
                column: "AdventureObjectId",
                principalTable: "AdventureObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureObjectGameStates_Games_GameId",
                table: "AdventureObjectGameStates",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureObjectLocation_AdventureObjects_AdventureObjectsId",
                table: "AdventureObjectLocation",
                column: "AdventureObjectsId",
                principalTable: "AdventureObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureObjectLocation_Locations_LocationsId",
                table: "AdventureObjectLocation",
                column: "LocationsId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureObjects_Adventures_AdventureId",
                table: "AdventureObjects",
                column: "AdventureId",
                principalTable: "Adventures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureObjects_Scripts_ActionScriptId",
                table: "AdventureObjects",
                column: "ActionScriptId",
                principalTable: "Scripts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureObjects_Scripts_InitializationScriptId",
                table: "AdventureObjects",
                column: "InitializationScriptId",
                principalTable: "Scripts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Adventures_Scripts_InitializationScriptId",
                table: "Adventures",
                column: "InitializationScriptId",
                principalTable: "Scripts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Adventures_Scripts_TerminationScriptId",
                table: "Adventures",
                column: "TerminationScriptId",
                principalTable: "Scripts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scripts_Adventures_AdventureId",
                table: "Scripts");

            migrationBuilder.DropTable(
                name: "AdventureObjectGameStates");

            migrationBuilder.DropTable(
                name: "AdventureObjectLocation");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "ScriptScript");

            migrationBuilder.DropTable(
                name: "SourcesEn");

            migrationBuilder.DropTable(
                name: "SourcesEsp");

            migrationBuilder.DropTable(
                name: "AdventureObjects");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Adventures");

            migrationBuilder.DropTable(
                name: "Scripts");
        }
    }
}
