using Microsoft.EntityFrameworkCore.Migrations;

namespace SuperPartner.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "P_Functions",
                columns: table => new
                {
                    FuncCode = table.Column<string>(maxLength: 50, nullable: false),
                    FuncName = table.Column<string>(maxLength: 200, nullable: true),
                    AssociateUrls = table.Column<string>(maxLength: 4000, nullable: true),
                    FuncDesc = table.Column<string>(type: "ntext", nullable: true),
                    ExtendProperties = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_P_Functions", x => x.FuncCode);
                });

            migrationBuilder.CreateTable(
                name: "P_User2Functions",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    FuncCode = table.Column<string>(maxLength: 50, nullable: false),
                    AccessLevel = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_P_User2Functions", x => new { x.UserId, x.FuncCode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "P_Functions");

            migrationBuilder.DropTable(
                name: "P_User2Functions");
        }
    }
}
