using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Changing_Team_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Members",
                newName: "Position");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Members",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Members",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Country", "Nationality", "Phone", "Position", "Summary" },
                values: new object[] { null, null, "+27814212260", 4, null });

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Country", "Name", "Nationality", "Phone", "Position", "Summary" },
                values: new object[] { null, "Johann", null, "+27824163759", 1, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Members",
                newName: "Department");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(70)",
                oldMaxLength: 70);

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 1,
                column: "Department",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Department", "Name" },
                values: new object[] { 3, "Annie" });
        }
    }
}
