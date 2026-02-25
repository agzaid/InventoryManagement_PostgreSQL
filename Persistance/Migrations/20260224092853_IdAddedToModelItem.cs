using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class IdAddedToModelItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.items DROP CONSTRAINT IF EXISTS ""PK_items"";
                ALTER TABLE kwarehouse.items DROP CONSTRAINT IF EXISTS items_pkey;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "catgry_code",
                schema: "kwarehouse",
                table: "items",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "items",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_items",
                schema: "kwarehouse",
                table: "items",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_items_catgry_code_item_code",
                schema: "kwarehouse",
                table: "items",
                columns: new[] { "catgry_code", "item_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.items DROP CONSTRAINT IF EXISTS ""PK_items"";
                ALTER TABLE kwarehouse.items DROP CONSTRAINT IF EXISTS items_pkey;
            ");

            migrationBuilder.DropIndex(
                name: "IX_items_catgry_code_item_code",
                schema: "kwarehouse",
                table: "items");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "items");

            migrationBuilder.AlterColumn<string>(
                name: "catgry_code",
                schema: "kwarehouse",
                table: "items",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_items",
                schema: "kwarehouse",
                table: "items",
                columns: new[] { "catgry_code", "item_code" });
        }
    }
}
