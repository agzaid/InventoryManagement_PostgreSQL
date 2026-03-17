using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class active : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                schema: "kwarehouse",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                schema: "kwarehouse",
                table: "user_roles");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "userroleassignments",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "users",
                schema: "kwarehouse");

            migrationBuilder.AlterColumn<string>(
                name: "role_id",
                schema: "kwarehouse",
                table: "user_roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                schema: "kwarehouse",
                table: "user_roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "isactive",
                schema: "kwarehouse",
                table: "aspnetroles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_aspnetroles_role_id",
                schema: "kwarehouse",
                table: "user_roles",
                column: "role_id",
                principalSchema: "kwarehouse",
                principalTable: "aspnetroles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_aspnetroles_role_id",
                schema: "kwarehouse",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "isactive",
                schema: "kwarehouse",
                table: "aspnetroles");

            migrationBuilder.AlterColumn<int>(
                name: "role_id",
                schema: "kwarehouse",
                table: "user_roles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                schema: "kwarehouse",
                table: "user_roles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "kwarehouse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "userroleassignments",
                schema: "kwarehouse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<string>(type: "text", nullable: true),
                    assignedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    assignedby = table.Column<string>(type: "text", nullable: true),
                    isgranted = table.Column<bool>(type: "boolean", nullable: false),
                    permissioncode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userroleassignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_userroleassignments_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalSchema: "kwarehouse",
                        principalTable: "aspnetroles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userroleassignments_aspnetusers_userid",
                        column: x => x.userid,
                        principalSchema: "kwarehouse",
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "kwarehouse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                schema: "kwarehouse",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userroleassignments_roleid",
                schema: "kwarehouse",
                table: "userroleassignments",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_userroleassignments_userid",
                schema: "kwarehouse",
                table: "userroleassignments",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "kwarehouse",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                schema: "kwarehouse",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                schema: "kwarehouse",
                table: "user_roles",
                column: "role_id",
                principalSchema: "kwarehouse",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                schema: "kwarehouse",
                table: "user_roles",
                column: "user_id",
                principalSchema: "kwarehouse",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
