using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class branch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.EnsureSchema(
            //    name: "kwarehouse");

            ////migrationBuilder.EnsureSchema(
            ////    name: "KWAREHOUSE");

            //migrationBuilder.CreateSequence<int>(
            //    name: "seq_item_category_code",
            //    schema: "kwarehouse");

            //migrationBuilder.CreateTable(
            //    name: "branches",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        branch_code = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        branch_desc = table.Column<string>(type: "text", nullable: true),
            //        branchdescarabic = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_branches", x => x.branch_code);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "departments",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        dep_code = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        dep_desc = table.Column<string>(type: "text", nullable: true),
            //        sector_code = table.Column<string>(type: "text", nullable: true),
            //        general_maneg = table.Column<string>(type: "text", nullable: true),
            //        maneg_supcode = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_departments", x => x.dep_code);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "emp_egx",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        dep_code = table.Column<int>(type: "integer", nullable: false),
            //        emp_code = table.Column<int>(type: "integer", nullable: false),
            //        emp_name = table.Column<string>(type: "text", nullable: true),
            //        branch_code = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_emp_egx", x => new { x.dep_code, x.emp_code });
            //        table.UniqueConstraint("AK_emp_egx_emp_code", x => x.emp_code);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "item_balance",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false),
            //        bal_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        item_code = table.Column<string>(type: "text", nullable: false),
            //        open_bal = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_in = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_out = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_from = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_to = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_back = table.Column<decimal>(type: "numeric", nullable: false),
            //        current_bal = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_back2 = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_scrap = table.Column<decimal>(type: "numeric", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_item_balance", x => new { x.store_code, x.bal_date, x.item_code });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "item_card",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false),
            //        item_code = table.Column<string>(type: "text", nullable: false),
            //        card_serial = table.Column<int>(type: "integer", nullable: false),
            //        item_desc = table.Column<string>(type: "text", nullable: true),
            //        card_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        card_memo = table.Column<string>(type: "text", nullable: true),
            //        in_num = table.Column<int>(type: "integer", nullable: true),
            //        in_qnt = table.Column<decimal>(type: "numeric", nullable: true),
            //        in_price = table.Column<decimal>(type: "numeric", nullable: true),
            //        out_num = table.Column<int>(type: "integer", nullable: true),
            //        out_qnt = table.Column<decimal>(type: "numeric", nullable: true),
            //        card_balance = table.Column<decimal>(type: "numeric", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_item_card", x => new { x.store_code, x.item_code, x.card_serial });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "item_category",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        catgry_code = table.Column<string>(type: "text", nullable: false),
            //        catgry_desc = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_item_category", x => x.catgry_code);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "monthly_balance",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false),
            //        bal_year = table.Column<int>(type: "integer", nullable: false),
            //        bal_month = table.Column<int>(type: "integer", nullable: false),
            //        item_code = table.Column<string>(type: "text", nullable: false),
            //        open_bal = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_in = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_out = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_from = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_to = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_back = table.Column<decimal>(type: "numeric", nullable: false),
            //        current_bal = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_back2 = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_scrap = table.Column<decimal>(type: "numeric", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_monthly_balance", x => new { x.store_code, x.bal_year, x.bal_month, x.item_code });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "monthly_consum",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false),
            //        consum_year = table.Column<int>(type: "integer", nullable: false),
            //        consum_month = table.Column<int>(type: "integer", nullable: false),
            //        dep_code = table.Column<int>(type: "integer", nullable: false),
            //        item_code = table.Column<string>(type: "text", nullable: false),
            //        consum_qnt = table.Column<decimal>(type: "numeric", nullable: false),
            //        consum_avg = table.Column<decimal>(type: "numeric", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_monthly_consum", x => new { x.store_code, x.consum_year, x.consum_month, x.dep_code, x.item_code });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "open_balance",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false),
            //        open_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        item_code = table.Column<string>(type: "text", nullable: false),
            //        open_bal = table.Column<decimal>(type: "numeric", nullable: false),
            //        relay_flag = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_open_balance", x => new { x.store_code, x.open_date, x.item_code });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "stores",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        store_desc = table.Column<string>(type: "text", nullable: true),
            //        sys_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        in_num = table.Column<int>(type: "integer", nullable: false),
            //        out_num = table.Column<int>(type: "integer", nullable: false),
            //        to_num = table.Column<int>(type: "integer", nullable: false),
            //        back_num = table.Column<int>(type: "integer", nullable: false),
            //        sys_lock = table.Column<string>(type: "text", nullable: true),
            //        back_num2 = table.Column<int>(type: "integer", nullable: false),
            //        scrap_num = table.Column<int>(type: "integer", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_stores", x => x.store_code);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "supplier",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        suplier_code = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        suplier_desc = table.Column<string>(type: "text", nullable: true),
            //        suplier_address = table.Column<string>(type: "text", nullable: true),
            //        suplier_tel = table.Column<string>(type: "text", nullable: true),
            //        suplier_fax = table.Column<string>(type: "text", nullable: true),
            //        suplier_email = table.Column<string>(type: "text", nullable: true),
            //        suplier_activity = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_supplier", x => x.suplier_code);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "sys_messg",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        msg_code = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        msg_desc = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_sys_messg", x => x.msg_code);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "inv_users",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        user_code = table.Column<int>(type: "integer", nullable: false),
            //        user_name = table.Column<string>(type: "text", nullable: true),
            //        user_passwd = table.Column<string>(type: "text", nullable: true),
            //        prog01 = table.Column<string>(type: "text", nullable: true),
            //        prog02 = table.Column<string>(type: "text", nullable: true),
            //        prog03 = table.Column<string>(type: "text", nullable: true),
            //        prog11 = table.Column<string>(type: "text", nullable: true),
            //        prog12 = table.Column<string>(type: "text", nullable: true),
            //        prog13 = table.Column<string>(type: "text", nullable: true),
            //        prog14 = table.Column<string>(type: "text", nullable: true),
            //        prog21 = table.Column<string>(type: "text", nullable: true),
            //        prog22 = table.Column<string>(type: "text", nullable: true),
            //        prog23 = table.Column<string>(type: "text", nullable: true),
            //        prog24 = table.Column<string>(type: "text", nullable: true),
            //        prog25 = table.Column<string>(type: "text", nullable: true),
            //        prog31 = table.Column<string>(type: "text", nullable: true),
            //        prog32 = table.Column<string>(type: "text", nullable: true),
            //        prog33 = table.Column<string>(type: "text", nullable: true),
            //        prog34 = table.Column<string>(type: "text", nullable: true),
            //        prog35 = table.Column<string>(type: "text", nullable: true),
            //        prog29 = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_inv_users", x => x.user_code);
            //        table.ForeignKey(
            //            name: "FK_inv_users_emp_egx_user_code",
            //            column: x => x.user_code,
            //            principalSchema: "kwarehouse",
            //            principalTable: "emp_egx",
            //            principalColumn: "emp_code",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "items",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        item_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false, defaultValueSql: "NULL"),
            //        catgry_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
            //        item_desc = table.Column<string>(type: "text", nullable: true),
            //        recall_prc = table.Column<decimal>(type: "numeric", nullable: true),
            //        recall_qnt = table.Column<decimal>(type: "numeric", nullable: true),
            //        barecode = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_items", x => new { x.catgry_code, x.item_code });
            //        table.UniqueConstraint("AK_items_item_code", x => x.item_code);
            //        table.ForeignKey(
            //            name: "FK_items_item_category_catgry_code",
            //            column: x => x.catgry_code,
            //            principalSchema: "kwarehouse",
            //            principalTable: "item_category",
            //            principalColumn: "catgry_code",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "h_inv_trans",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false),
            //        tr_type = table.Column<int>(type: "integer", nullable: false),
            //        tr_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        tr_num = table.Column<int>(type: "integer", nullable: false),
            //        tr_serial = table.Column<int>(type: "integer", nullable: false),
            //        item_code = table.Column<string>(type: "character varying(5)", nullable: false),
            //        item_qnt = table.Column<decimal>(type: "numeric", nullable: false),
            //        dep_code = table.Column<int>(type: "integer", nullable: true),
            //        emp_code = table.Column<int>(type: "integer", nullable: true),
            //        suplier_code = table.Column<int>(type: "integer", nullable: true),
            //        from_to_store = table.Column<int>(type: "integer", nullable: true),
            //        item_price = table.Column<decimal>(type: "numeric", nullable: true),
            //        bill_num = table.Column<int>(type: "integer", nullable: true),
            //        tr_num2 = table.Column<int>(type: "integer", nullable: true),
            //        tr_date2 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        deliver_no = table.Column<int>(type: "integer", nullable: true),
            //        deliver_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_h_inv_trans", x => new { x.store_code, x.tr_type, x.tr_date, x.tr_num, x.tr_serial, x.item_code });
            //        table.ForeignKey(
            //            name: "FK_h_inv_trans_items_item_code",
            //            column: x => x.item_code,
            //            principalSchema: "kwarehouse",
            //            principalTable: "items",
            //            principalColumn: "item_code",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_h_inv_trans_supplier_suplier_code",
            //            column: x => x.suplier_code,
            //            principalSchema: "kwarehouse",
            //            principalTable: "supplier",
            //            principalColumn: "suplier_code");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "inv_trans",
            //    schema: "kwarehouse",
            //    columns: table => new
            //    {
            //        store_code = table.Column<int>(type: "integer", nullable: false),
            //        tr_type = table.Column<int>(type: "integer", nullable: false),
            //        tr_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        tr_num = table.Column<int>(type: "integer", nullable: false),
            //        tr_serial = table.Column<int>(type: "integer", nullable: false),
            //        item_code = table.Column<string>(type: "character varying(5)", nullable: false),
            //        dep_code = table.Column<int>(type: "integer", nullable: true),
            //        emp_code = table.Column<int>(type: "integer", nullable: true),
            //        suplier_code = table.Column<int>(type: "integer", nullable: true),
            //        from_to_store = table.Column<int>(type: "integer", nullable: true),
            //        item_qnt = table.Column<decimal>(type: "numeric", nullable: false),
            //        item_price = table.Column<decimal>(type: "numeric", nullable: true),
            //        bill_num = table.Column<int>(type: "integer", nullable: true),
            //        tr_num2 = table.Column<int>(type: "integer", nullable: true),
            //        tr_date2 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        deliver_no = table.Column<int>(type: "integer", nullable: true),
            //        deliver_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_inv_trans", x => new { x.store_code, x.tr_type, x.tr_date, x.tr_num, x.tr_serial, x.item_code });
            //        table.ForeignKey(
            //            name: "FK_inv_trans_departments_dep_code",
            //            column: x => x.dep_code,
            //            principalSchema: "kwarehouse",
            //            principalTable: "departments",
            //            principalColumn: "dep_code");
            //        table.ForeignKey(
            //            name: "FK_inv_trans_emp_egx_dep_code_emp_code",
            //            columns: x => new { x.dep_code, x.emp_code },
            //            principalSchema: "kwarehouse",
            //            principalTable: "emp_egx",
            //            principalColumns: new[] { "dep_code", "emp_code" });
            //        table.ForeignKey(
            //            name: "FK_inv_trans_items_item_code",
            //            column: x => x.item_code,
            //            principalSchema: "kwarehouse",
            //            principalTable: "items",
            //            principalColumn: "item_code",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_inv_trans_supplier_suplier_code",
            //            column: x => x.suplier_code,
            //            principalSchema: "kwarehouse",
            //            principalTable: "supplier",
            //            principalColumn: "suplier_code");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_h_inv_trans_item_code",
            //    schema: "kwarehouse",
            //    table: "h_inv_trans",
            //    column: "item_code");

            //migrationBuilder.CreateIndex(
            //    name: "IX_h_inv_trans_suplier_code",
            //    schema: "kwarehouse",
            //    table: "h_inv_trans",
            //    column: "suplier_code");

            //migrationBuilder.CreateIndex(
            //    name: "IX_inv_trans_dep_code_emp_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    columns: new[] { "dep_code", "emp_code" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_inv_trans_item_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    column: "item_code");

            //migrationBuilder.CreateIndex(
            //    name: "IX_inv_trans_suplier_code",
            //    schema: "kwarehouse",
            //    table: "inv_trans",
            //    column: "suplier_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branches",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "h_inv_trans",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "inv_trans",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "inv_users",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "item_balance",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "item_card",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "monthly_balance",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "monthly_consum",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "open_balance",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "stores",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "sys_messg",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "items",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "supplier",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "emp_egx",
                schema: "kwarehouse");

            migrationBuilder.DropTable(
                name: "item_category",
                schema: "kwarehouse");

            migrationBuilder.DropSequence(
                name: "seq_item_category_code",
                schema: "kwarehouse");
        }
    }
}
