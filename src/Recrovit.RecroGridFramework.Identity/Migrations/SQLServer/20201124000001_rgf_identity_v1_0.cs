using Microsoft.EntityFrameworkCore.Migrations;

namespace Recrovit.RecroGridFramework.Identity.Migrations.SQLServer
{
    public partial class rgf_identity_v1_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RGF_IdentityRole",
                columns: table => new
                {
                    RoleId = table.Column<string>(maxLength: 255, nullable: false),
                    RoleName = table.Column<string>(maxLength: 255, nullable: false),
                    RoleScope = table.Column<string>(maxLength: 255, nullable: true),
                    Source = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RGF_IdentityRole", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "RGF_IdentityUser",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 255, nullable: false),
                    UserName = table.Column<string>(maxLength: 255, nullable: false),
                    Language = table.Column<string>(maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RGF_IdentityUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RGF_IdentityUserRole",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 255, nullable: false),
                    RoleId = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RGF_IdentityUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RGF_IdentityUserRole_RGF_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RGF_IdentityRole",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RGF_IdentityUserRole_RGF_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "RGF_IdentityUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RGF_IdentityRole",
                columns: new[] { "RoleId", "RoleName", "RoleScope", "Source" },
                values: new object[] { "1", "Administrators", null, "RGF" });

            migrationBuilder.InsertData(
                table: "RGF_IdentityRole",
                columns: new[] { "RoleId", "RoleName", "RoleScope", "Source" },
                values: new object[] { "2", "Users", null, "RGF" });

            migrationBuilder.CreateIndex(
                name: "IX_RGF_IdentityUserRole_RoleId",
                table: "RGF_IdentityUserRole",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RGF_IdentityUserRole");

            migrationBuilder.DropTable(
                name: "RGF_IdentityRole");

            migrationBuilder.DropTable(
                name: "RGF_IdentityUser");
        }
    }
}
