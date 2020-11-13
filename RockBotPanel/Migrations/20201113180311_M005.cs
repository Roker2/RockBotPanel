using Microsoft.EntityFrameworkCore.Migrations;

namespace RockBotPanel.Migrations
{
    public partial class M005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastValidationCode",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastValidationCode",
                table: "AspNetUsers");
        }
    }
}
