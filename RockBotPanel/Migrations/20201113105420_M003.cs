using Microsoft.EntityFrameworkCore.Migrations;

namespace RockBotPanel.Migrations
{
    public partial class M003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatIds",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Chatinfo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarnsQuantity = table.Column<int>(nullable: true),
                    Welcome = table.Column<string>(nullable: true),
                    Rules = table.Column<string>(nullable: true),
                    DisabledCommands = table.Column<string>(nullable: true),
                    TelegramUserId = table.Column<string>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chatinfo");

            migrationBuilder.AddColumn<string>(
                name: "ChatIds",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
