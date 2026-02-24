using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class invtransModelIdAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_departments_depcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_items_itemcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS ""PK_inv_trans"";
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS inv_trans_pkey;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "employeeid",
                schema: "kwarehouse",
                table: "inv_trans",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "itemcode",
                schema: "kwarehouse",
                table: "inv_trans",
                type: "character varying(5)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "inv_trans",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_inv_trans",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_departments_dep_code",
                schema: "kwarehouse",
                table: "departments",
                column: "dep_code");

            migrationBuilder.CreateIndex(
                name: "IX_inv_trans_storecode_trtype_trdate_trnum_trserial_itemcode",
                schema: "kwarehouse",
                table: "inv_trans",
                columns: new[] { "storecode", "trtype", "trdate", "trnum", "trserial", "itemcode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_departments_depcode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "depcode",
                principalSchema: "kwarehouse",
                principalTable: "departments",
                principalColumn: "dep_code");

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_items_itemcode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "itemcode",
                principalSchema: "kwarehouse",
                principalTable: "items",
                principalColumn: "item_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_departments_depcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_items_itemcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS ""PK_inv_trans"";
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS inv_trans_pkey;
            ");

            migrationBuilder.DropIndex(
                name: "IX_inv_trans_storecode_trtype_trdate_trnum_trserial_itemcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_departments_dep_code",
                schema: "kwarehouse",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.AlterColumn<string>(
                name: "itemcode",
                schema: "kwarehouse",
                table: "inv_trans",
                type: "character varying(5)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "employeeid",
                schema: "kwarehouse",
                table: "inv_trans",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_inv_trans",
                schema: "kwarehouse",
                table: "inv_trans",
                columns: new[] { "storecode", "trtype", "trdate", "trnum", "trserial", "itemcode" });

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_departments_depcode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "depcode",
                principalSchema: "kwarehouse",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_items_itemcode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "itemcode",
                principalSchema: "kwarehouse",
                principalTable: "items",
                principalColumn: "item_code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
