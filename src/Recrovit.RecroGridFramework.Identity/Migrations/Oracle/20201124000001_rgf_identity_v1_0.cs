using Microsoft.EntityFrameworkCore.Migrations;

namespace Recrovit.RecroGridFramework.Identity.Migrations.Oracle
{
    public partial class rgf_identity_v1_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RGF_IDENTITYROLE",
                columns: table => new
                {
                    ROLEID = table.Column<string>(maxLength: 255, nullable: false),
                    ROLENAME = table.Column<string>(maxLength: 255, nullable: false),
                    ROLESCOPE = table.Column<string>(maxLength: 255, nullable: true),
                    SOURCE = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RGF_IDENTITYROLE", x => x.ROLEID);
                });

            migrationBuilder.CreateTable(
                name: "RGF_IDENTITYUSER",
                columns: table => new
                {
                    USERID = table.Column<string>(maxLength: 255, nullable: false),
                    USERNAME = table.Column<string>(maxLength: 255, nullable: false),
                    LANGUAGE = table.Column<string>(maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RGF_IDENTITYUSER", x => x.USERID);
                });

            migrationBuilder.CreateTable(
                name: "RGF_IDENTITYUSERROLE",
                columns: table => new
                {
                    USERID = table.Column<string>(maxLength: 255, nullable: false),
                    ROLEID = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RGF_IDENTITYUSERROLE", x => new { x.USERID, x.ROLEID });
                    table.ForeignKey(
                        name: "FK_RGF_IDENTITYUSERROLE_RGF__1",
                        column: x => x.ROLEID,
                        principalTable: "RGF_IDENTITYROLE",
                        principalColumn: "ROLEID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RGF_IDENTITYUSERROLE_RGF__2",
                        column: x => x.USERID,
                        principalTable: "RGF_IDENTITYUSER",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RGF_IDENTITYROLE",
                columns: new[] { "ROLEID", "ROLENAME", "ROLESCOPE", "SOURCE" },
                values: new object[] { "1", "Administrators", null, "RGF" });

            migrationBuilder.InsertData(
                table: "RGF_IDENTITYROLE",
                columns: new[] { "ROLEID", "ROLENAME", "ROLESCOPE", "SOURCE" },
                values: new object[] { "2", "Users", null, "RGF" });

            migrationBuilder.CreateIndex(
                name: "IX_RGF_IDENTITYUSERROLE_ROLEID",
                table: "RGF_IDENTITYUSERROLE",
                column: "ROLEID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RGF_IDENTITYUSERROLE");

            migrationBuilder.DropTable(
                name: "RGF_IDENTITYROLE");

            migrationBuilder.DropTable(
                name: "RGF_IDENTITYUSER");
        }
    }
}
