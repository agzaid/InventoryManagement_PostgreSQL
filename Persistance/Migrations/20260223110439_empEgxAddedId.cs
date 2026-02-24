using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class empEgxAddedId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_departments_dep_code",
                schema: "kwarehouse",
                table: "inv_trans");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_inv_trans_emp_egx_dep_code_emp_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_inv_trans_items_item_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_inv_trans_supplier_suplier_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans");

            //migrationBuilder.DropIndex(
            //    name: "IX_inv_trans_dep_code_emp_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_emp_egx",
            //    schema: "kwarehouse",
            //    table: "emp_egx");

            migrationBuilder.RenameColumn(
                name: "tr_num2",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "trnum2");

            migrationBuilder.RenameColumn(
                name: "tr_date2",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "trdate2");

            migrationBuilder.RenameColumn(
                name: "suplier_code",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "supliercode");

            migrationBuilder.RenameColumn(
                name: "order_date",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "orderdate");

            migrationBuilder.RenameColumn(
                name: "item_qnt",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "itemqnt");

            migrationBuilder.RenameColumn(
                name: "item_price",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "itemprice");

            migrationBuilder.RenameColumn(
                name: "from_to_store",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "fromtostore");

            migrationBuilder.RenameColumn(
                name: "emp_code",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "empcode");

            migrationBuilder.RenameColumn(
                name: "dep_code",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "depcode");

            migrationBuilder.RenameColumn(
                name: "deliver_no",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "deliverno");

            migrationBuilder.RenameColumn(
                name: "deliver_date",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "deliverdate");

            migrationBuilder.RenameColumn(
                name: "bill_num",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "billnum");

            migrationBuilder.RenameColumn(
                name: "item_code",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "itemcode");

            migrationBuilder.RenameColumn(
                name: "tr_serial",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "trserial");

            migrationBuilder.RenameColumn(
                name: "tr_num",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "trnum");

            migrationBuilder.RenameColumn(
                name: "tr_date",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "trdate");

            migrationBuilder.RenameColumn(
                name: "tr_type",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "trtype");

            migrationBuilder.RenameColumn(
                name: "store_code",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "storecode");

            //migrationBuilder.RenameIndex(
            //    name: "IX_inv_trans_suplier_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    newName: "IX_inv_trans_supliercode");

            //migrationBuilder.RenameIndex(
            //    name: "IX_inv_trans_item_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    newName: "IX_inv_trans_itemcode");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "emp_egx",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_emp_egx",
            //    schema: "kwarehouse",
            //    table: "emp_egx",
            //    column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_emp_egx_id",
                schema: "kwarehouse",
                table: "emp_egx",
                column: "id",
                unique: true);

            migrationBuilder.AddColumn<int>(
                name: "employeeid",
                schema: "kwarehouse",
                table: "inv_trans",
                type: "integer",
                nullable: true);
            migrationBuilder.Sql(@"
                    UPDATE kwarehouse.inv_trans t
                    SET employeeid = e.id
                    FROM kwarehouse.emp_egx e
                    WHERE t.depcode = e.dep_code AND t.empcode = e.emp_code;
                ");

            migrationBuilder.CreateIndex(
                name: "IX_inv_trans_depcode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "depcode");

            migrationBuilder.CreateIndex(
                name: "IX_inv_trans_employeeid",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_emp_egx_dep_code_emp_code",
                schema: "kwarehouse",
                table: "emp_egx",
                columns: new[] { "dep_code", "emp_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_item_code",
                schema: "kwarehouse",
                table: "items",
                column: "item_code",
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
                name: "FK_inv_trans_emp_egx_employeeid",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "employeeid",
                principalSchema: "kwarehouse",
                principalTable: "emp_egx",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_items_itemcode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "itemcode",
                principalSchema: "kwarehouse",
                principalTable: "items",
                principalColumn: "item_code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"
                    UPDATE kwarehouse.inv_trans t
                    SET supliercode = NULL
                    WHERE supliercode IS NOT NULL
                      AND NOT EXISTS (
                          SELECT 1
                          FROM kwarehouse.supplier s
                          WHERE s.suplier_code = t.supliercode
                      );
                ");

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_supplier_supliercode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "supliercode",
                principalSchema: "kwarehouse",
                principalTable: "supplier",
                principalColumn: "suplier_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_departments_depcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_emp_egx_employeeid",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_items_itemcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropForeignKey(
                name: "FK_inv_trans_supplier_supliercode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropIndex(
                name: "IX_inv_trans_depcode",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropIndex(
                name: "IX_inv_trans_employeeid",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropIndex(
                name: "IX_emp_egx_id",
                schema: "kwarehouse",
                table: "emp_egx");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_emp_egx",
            //    schema: "kwarehouse",
            //    table: "emp_egx");

            migrationBuilder.DropIndex(
                name: "IX_emp_egx_dep_code_emp_code",
                schema: "kwarehouse",
                table: "emp_egx");

            migrationBuilder.DropIndex(
                name: "IX_items_item_code",
                schema: "kwarehouse",
                table: "items");

            migrationBuilder.DropColumn(
                name: "employeeid",
                schema: "kwarehouse",
                table: "inv_trans");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "emp_egx");

            migrationBuilder.RenameColumn(
                name: "trnum2",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "tr_num2");

            migrationBuilder.RenameColumn(
                name: "trdate2",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "tr_date2");

            migrationBuilder.RenameColumn(
                name: "supliercode",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "suplier_code");

            migrationBuilder.RenameColumn(
                name: "orderdate",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "order_date");

            migrationBuilder.RenameColumn(
                name: "itemqnt",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "item_qnt");

            migrationBuilder.RenameColumn(
                name: "itemprice",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "item_price");

            migrationBuilder.RenameColumn(
                name: "fromtostore",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "from_to_store");

            migrationBuilder.RenameColumn(
                name: "empcode",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "emp_code");

            migrationBuilder.RenameColumn(
                name: "depcode",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "dep_code");

            migrationBuilder.RenameColumn(
                name: "deliverno",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "deliver_no");

            migrationBuilder.RenameColumn(
                name: "deliverdate",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "deliver_date");

            migrationBuilder.RenameColumn(
                name: "billnum",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "bill_num");

            migrationBuilder.RenameColumn(
                name: "itemcode",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "item_code");

            migrationBuilder.RenameColumn(
                name: "trserial",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "tr_serial");

            migrationBuilder.RenameColumn(
                name: "trnum",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "tr_num");

            migrationBuilder.RenameColumn(
                name: "trdate",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "tr_date");

            migrationBuilder.RenameColumn(
                name: "trtype",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "tr_type");

            migrationBuilder.RenameColumn(
                name: "storecode",
                schema: "kwarehouse",
                table: "inv_trans",
                newName: "store_code");

            //migrationBuilder.RenameIndex(
            //    name: "IX_inv_trans_supliercode",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    newName: "IX_inv_trans_suplier_code");

            //migrationBuilder.RenameIndex(
            //    name: "IX_inv_trans_itemcode",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    newName: "IX_inv_trans_item_code");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_emp_egx",
            //    schema: "kwarehouse",
            //    table: "emp_egx",
            //    columns: new[] { "dep_code", "emp_code" });

            migrationBuilder.CreateIndex(
                name: "IX_inv_trans_dep_code_emp_code",
                schema: "kwarehouse",
                table: "inv_trans",
                columns: new[] { "dep_code", "emp_code" });

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_departments_dep_code",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "dep_code",
                principalSchema: "kwarehouse",
                principalTable: "departments",
                principalColumn: "dep_code");

            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_emp_egx_dep_code_emp_code",
                schema: "kwarehouse",
                table: "inv_trans",
                columns: new[] { "dep_code", "emp_code" },
                principalSchema: "kwarehouse",
                principalTable: "emp_egx",
                principalColumns: new[] { "dep_code", "emp_code" });

            //migrationBuilder.AddForeignKey(
            //    name: "FK_inv_trans_items_item_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    column: "item_code",
            //    principalSchema: "kwarehouse",
            //    principalTable: "items",
            //    principalColumn: "item_code",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_inv_trans_supplier_suplier_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    column: "suplier_code",
            //    principalSchema: "kwarehouse",
            //    principalTable: "supplier",
            //    principalColumn: "suplier_code");
        }
    }
}
