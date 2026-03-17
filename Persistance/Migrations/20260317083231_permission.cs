using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class permission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissionactions",
                schema: "kwarehouse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    permissioncode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    permissionname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    module = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    controller = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fullurl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    httpmethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "GET"),
                    description = table.Column<string>(type: "text", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissionactions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_permissionactions_permissioncode_controller_action",
                schema: "kwarehouse",
                table: "permissionactions",
                columns: new[] { "permissioncode", "controller", "action" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permissionactions",
                schema: "kwarehouse");
        }
    }
}
