using Microsoft.EntityFrameworkCore.Migrations;

namespace dashboard.Migrations
{
    public partial class removecodecolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "employees");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
