using Microsoft.EntityFrameworkCore.Migrations;

namespace RockBotPanel.Migrations
{
    public partial class M004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chatinfo");

            migrationBuilder.AddColumn<string>(
                name: "ChatIds",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatIds",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Chatinfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisabledCommands = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelegramUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WarnsQuantity = table.Column<int>(type: "int", nullable: true),
                    Welcome = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatinfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chatinfo_AspNetUsers_TelegramUserId",
                        column: x => x.TelegramUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chatinfo_TelegramUserId",
                table: "Chatinfo",
                column: "TelegramUserId");
        }
    }
}
