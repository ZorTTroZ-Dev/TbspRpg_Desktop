using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbspRpgDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdventureFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InitialSourceKey",
                table: "Adventures",
                newName: "InitialCopyKey");

            migrationBuilder.RenameColumn(
                name: "DescriptionSourceKey",
                table: "Adventures",
                newName: "DescriptionCopyKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InitialCopyKey",
                table: "Adventures",
                newName: "InitialSourceKey");

            migrationBuilder.RenameColumn(
                name: "DescriptionCopyKey",
                table: "Adventures",
                newName: "DescriptionSourceKey");
        }
    }
}
