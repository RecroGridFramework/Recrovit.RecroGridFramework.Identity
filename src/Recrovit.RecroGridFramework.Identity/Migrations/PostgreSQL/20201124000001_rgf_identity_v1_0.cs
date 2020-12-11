using Microsoft.EntityFrameworkCore.Migrations;

namespace Recrovit.RecroGridFramework.Identity.Migrations.PostgreSQL
{
    public partial class rgf_identity_v1_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "rgf_identityrole",
                schema: "public",
                columns: table => new
                {
                    roleid = table.Column<string>(maxLength: 255, nullable: false),
                    rolename = table.Column<string>(maxLength: 255, nullable: false),
                    rolescope = table.Column<string>(maxLength: 255, nullable: true),
                    source = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rgf_identityrole", x => x.roleid);
                });

            migrationBuilder.CreateTable(
                name: "rgf_identityuser",
                schema: "public",
                columns: table => new
                {
                    userid = table.Column<string>(maxLength: 255, nullable: false),
                    username = table.Column<string>(maxLength: 255, nullable: false),
                    language = table.Column<string>(maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rgf_identityuser", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "rgf_identityuserrole",
                schema: "public",
                columns: table => new
                {
                    userid = table.Column<string>(maxLength: 255, nullable: false),
                    roleid = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rgf_identityuserrole", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_rgf_identityuserrole_rgf_identityrole_roleid",
                        column: x => x.roleid,
                        principalSchema: "public",
                        principalTable: "rgf_identityrole",
                        principalColumn: "roleid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rgf_identityuserrole_rgf_identityuser_userid",
                        column: x => x.userid,
                        principalSchema: "public",
                        principalTable: "rgf_identityuser",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "rgf_identityrole",
                columns: new[] { "roleid", "rolename", "rolescope", "source" },
                values: new object[,]
                {
                    { "1", "Administrators", null, "RGF" },
                    { "2", "Users", null, "RGF" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_rgf_identityuserrole_roleid",
                schema: "public",
                table: "rgf_identityuserrole",
                column: "roleid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rgf_identityuserrole",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rgf_identityrole",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rgf_identityuser",
                schema: "public");
        }
    }
}
