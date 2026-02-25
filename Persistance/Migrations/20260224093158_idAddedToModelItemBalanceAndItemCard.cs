using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class idAddedToModelItemBalanceAndItemCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS ""PK_item_card"";
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS item_card_pkey;
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS ""PK_item_balance"";
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS item_balance_pkey;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "card_serial",
                schema: "kwarehouse",
                table: "item_card",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "item_card",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "store_code",
                schema: "kwarehouse",
                table: "item_card",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "item_card",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "item_balance",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "item_balance",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_card",
                schema: "kwarehouse",
                table: "item_card",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_balance",
                schema: "kwarehouse",
                table: "item_balance",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_item_card_store_code_item_code_card_serial",
                schema: "kwarehouse",
                table: "item_card",
                columns: new[] { "store_code", "item_code", "card_serial" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_balance_store_code_bal_date_item_code",
                schema: "kwarehouse",
                table: "item_balance",
                columns: new[] { "store_code", "bal_date", "item_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS ""PK_item_card"";
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS item_card_pkey;
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS ""PK_item_balance"";
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS item_balance_pkey;
            ");

            migrationBuilder.DropIndex(
                name: "IX_item_balance_store_code_bal_date_item_code",
                schema: "kwarehouse",
                table: "item_balance");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "item_card");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "item_balance");

            migrationBuilder.AlterColumn<int>(
                name: "store_code",
                schema: "kwarehouse",
                table: "item_card",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "item_card",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "card_serial",
                schema: "kwarehouse",
                table: "item_card",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "item_balance",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_card",
                schema: "kwarehouse",
                table: "item_card",
                columns: new[] { "store_code", "item_code", "card_serial" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_balance",
                schema: "kwarehouse",
                table: "item_balance",
                columns: new[] { "store_code", "bal_date", "item_code" });
        }
    }
}
