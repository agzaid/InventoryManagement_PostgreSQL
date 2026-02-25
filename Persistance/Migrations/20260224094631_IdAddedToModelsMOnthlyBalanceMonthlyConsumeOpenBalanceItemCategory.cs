using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class IdAddedToModelsMOnthlyBalanceMonthlyConsumeOpenBalanceItemCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS ""PK_open_balance"";
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS open_balance_pkey;
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS ""PK_monthly_consum"";
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS monthly_consum_pkey;
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS ""PK_monthly_balance"";
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS monthly_balance_pkey;
                ALTER TABLE kwarehouse.item_category DROP CONSTRAINT IF EXISTS ""PK_item_category"";
                ALTER TABLE kwarehouse.item_category DROP CONSTRAINT IF EXISTS item_category_pkey;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "open_balance",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "open_balance",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "monthly_consum",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "monthly_consum",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "monthly_balance",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "monthly_balance",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "item_category",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_open_balance",
                schema: "kwarehouse",
                table: "open_balance",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_monthly_consum",
                schema: "kwarehouse",
                table: "monthly_consum",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_monthly_balance",
                schema: "kwarehouse",
                table: "monthly_balance",
                column: "id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_item_category_catgry_code",
                schema: "kwarehouse",
                table: "item_category",
                column: "catgry_code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_category",
                schema: "kwarehouse",
                table: "item_category",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_open_balance_store_code_open_date_item_code",
                schema: "kwarehouse",
                table: "open_balance",
                columns: new[] { "store_code", "open_date", "item_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_monthly_consum_store_code_consum_year_consum_month_dep_code~",
                schema: "kwarehouse",
                table: "monthly_consum",
                columns: new[] { "store_code", "consum_year", "consum_month", "dep_code", "item_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_monthly_balance_store_code_bal_year_bal_month_item_code",
                schema: "kwarehouse",
                table: "monthly_balance",
                columns: new[] { "store_code", "bal_year", "bal_month", "item_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_category_catgry_code",
                schema: "kwarehouse",
                table: "item_category",
                column: "catgry_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS ""PK_open_balance"";
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS open_balance_pkey;
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS ""PK_monthly_consum"";
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS monthly_consum_pkey;
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS ""PK_monthly_balance"";
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS monthly_balance_pkey;
                ALTER TABLE kwarehouse.item_category DROP CONSTRAINT IF EXISTS ""PK_item_category"";
                ALTER TABLE kwarehouse.item_category DROP CONSTRAINT IF EXISTS item_category_pkey;
            ");

            migrationBuilder.DropIndex(
                name: "IX_item_category_catgry_code",
                schema: "kwarehouse",
                table: "item_category");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "open_balance");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "monthly_consum");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "monthly_balance");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "item_category");

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "open_balance",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "monthly_consum",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "item_code",
                schema: "kwarehouse",
                table: "monthly_balance",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_open_balance",
                schema: "kwarehouse",
                table: "open_balance",
                columns: new[] { "store_code", "open_date", "item_code" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_monthly_consum",
                schema: "kwarehouse",
                table: "monthly_consum",
                columns: new[] { "store_code", "consum_year", "consum_month", "dep_code", "item_code" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_monthly_balance",
                schema: "kwarehouse",
                table: "monthly_balance",
                columns: new[] { "store_code", "bal_year", "bal_month", "item_code" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_category",
                schema: "kwarehouse",
                table: "item_category",
                column: "catgry_code");
        }
    }
}
