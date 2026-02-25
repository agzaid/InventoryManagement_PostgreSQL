using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdToModelStoreSupplier3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop ALL foreign keys that might reference supplier and stores tables first
            migrationBuilder.Sql(@"
                -- Drop foreign keys from h_inv_trans
                ALTER TABLE kwarehouse.h_inv_trans DROP CONSTRAINT IF EXISTS FK_h_inv_trans_supplier_suplier_code;
                ALTER TABLE kwarehouse.h_inv_trans DROP CONSTRAINT IF EXISTS FK_h_inv_trans_stores_store_code;
                
                -- Drop foreign keys from inv_trans (EXACT names from database)
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS FK_inv_trans_supplier_supliercode;
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS FK_inv_trans_stores_storecode;
                
                -- Drop any other potential foreign keys to supplier
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS FK_item_balance_supplier_suplier_code;
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS FK_monthly_balance_supplier_suplier_code;
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS FK_monthly_consum_supplier_suplier_code;
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS FK_open_balance_supplier_suplier_code;
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS FK_item_card_supplier_suplier_code;
                
                -- Drop any other potential foreign keys to stores
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS FK_item_balance_stores_store_code;
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS FK_monthly_balance_stores_store_code;
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS FK_monthly_consum_stores_store_code;
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS FK_open_balance_stores_store_code;
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS FK_item_card_stores_store_code;
            ");

            // Drop primary keys with CASCADE to remove dependent foreign keys
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.supplier DROP CONSTRAINT IF EXISTS PK_supplier CASCADE;
                ALTER TABLE kwarehouse.supplier DROP CONSTRAINT IF EXISTS supplier_pkey CASCADE;
                ALTER TABLE kwarehouse.stores DROP CONSTRAINT IF EXISTS PK_stores CASCADE;
                ALTER TABLE kwarehouse.stores DROP CONSTRAINT IF EXISTS stores_pkey CASCADE;
            ");

            // Remove identity annotations from old primary key columns
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.supplier ALTER COLUMN suplier_code DROP IDENTITY IF EXISTS;
                ALTER TABLE kwarehouse.stores ALTER COLUMN store_code DROP IDENTITY IF EXISTS;
            ");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "supplier",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "stores",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_supplier_suplier_code",
                schema: "kwarehouse",
                table: "supplier",
                column: "suplier_code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_supplier",
                schema: "kwarehouse",
                table: "supplier",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stores",
                schema: "kwarehouse",
                table: "stores",
                column: "id");

            // Clean up invalid foreign key references before recreating constraints
            migrationBuilder.Sql(@"
                -- Clean up supplier references in h_inv_trans
                UPDATE kwarehouse.h_inv_trans t
                SET suplier_code = NULL
                WHERE suplier_code IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM kwarehouse.supplier s
                      WHERE s.suplier_code = t.suplier_code
                  );

                -- Clean up supplier references in inv_trans  
                UPDATE kwarehouse.inv_trans t
                SET supliercode = NULL
                WHERE supliercode IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM kwarehouse.supplier s
                      WHERE s.suplier_code = t.supliercode
                  );

                -- Clean up store references in h_inv_trans
                UPDATE kwarehouse.h_inv_trans t
                SET store_code = NULL
                WHERE store_code IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM kwarehouse.stores s
                      WHERE s.store_code = t.store_code
                  );

                -- Clean up store references in inv_trans
                UPDATE kwarehouse.inv_trans t
                SET storecode = NULL
                WHERE storecode IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM kwarehouse.stores s
                      WHERE s.store_code = t.storecode
                  );
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_h_inv_trans_supplier_suplier_code",
                schema: "kwarehouse",
                table: "h_inv_trans",
                column: "suplier_code",
                principalSchema: "kwarehouse",
                principalTable: "supplier",
                principalColumn: "suplier_code");

            // Add missing foreign key for inv_trans -> supplier
            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_supplier_supliercode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "supliercode",
                principalSchema: "kwarehouse",
                principalTable: "supplier",
                principalColumn: "suplier_code");

            // Add missing foreign key for h_inv_trans -> stores
            migrationBuilder.AddForeignKey(
                name: "FK_h_inv_trans_stores_store_code",
                schema: "kwarehouse",
                table: "h_inv_trans",
                column: "store_code",
                principalSchema: "kwarehouse",
                principalTable: "stores",
                principalColumn: "store_code");

            // Add missing foreign key for inv_trans -> stores
            migrationBuilder.AddForeignKey(
                name: "FK_inv_trans_stores_storecode",
                schema: "kwarehouse",
                table: "inv_trans",
                column: "storecode",
                principalSchema: "kwarehouse",
                principalTable: "stores",
                principalColumn: "store_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop ALL foreign keys with safe checks
            migrationBuilder.Sql(@"
                -- Drop foreign keys from h_inv_trans
                ALTER TABLE kwarehouse.h_inv_trans DROP CONSTRAINT IF EXISTS FK_h_inv_trans_supplier_suplier_code;
                ALTER TABLE kwarehouse.h_inv_trans DROP CONSTRAINT IF EXISTS FK_h_inv_trans_stores_store_code;
                
                -- Drop foreign keys from inv_trans (EXACT names from database)
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS FK_inv_trans_supplier_supliercode;
                ALTER TABLE kwarehouse.inv_trans DROP CONSTRAINT IF EXISTS FK_inv_trans_stores_storecode;
                
                -- Drop any other potential foreign keys to supplier
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS FK_item_balance_supplier_suplier_code;
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS FK_monthly_balance_supplier_suplier_code;
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS FK_monthly_consum_supplier_suplier_code;
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS FK_open_balance_supplier_suplier_code;
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS FK_item_card_supplier_suplier_code;
                
                -- Drop any other potential foreign keys to stores
                ALTER TABLE kwarehouse.item_balance DROP CONSTRAINT IF EXISTS FK_item_balance_stores_store_code;
                ALTER TABLE kwarehouse.monthly_balance DROP CONSTRAINT IF EXISTS FK_monthly_balance_stores_store_code;
                ALTER TABLE kwarehouse.monthly_consum DROP CONSTRAINT IF EXISTS FK_monthly_consum_stores_store_code;
                ALTER TABLE kwarehouse.open_balance DROP CONSTRAINT IF EXISTS FK_open_balance_stores_store_code;
                ALTER TABLE kwarehouse.item_card DROP CONSTRAINT IF EXISTS FK_item_card_stores_store_code;
            ");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_supplier_suplier_code",
                schema: "kwarehouse",
                table: "supplier");

            // Drop primary keys with CASCADE to remove dependent foreign keys
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.supplier DROP CONSTRAINT IF EXISTS PK_supplier CASCADE;
                ALTER TABLE kwarehouse.supplier DROP CONSTRAINT IF EXISTS supplier_pkey CASCADE;
                ALTER TABLE kwarehouse.stores DROP CONSTRAINT IF EXISTS PK_stores CASCADE;
                ALTER TABLE kwarehouse.stores DROP CONSTRAINT IF EXISTS stores_pkey CASCADE;
            ");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "supplier");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "stores");

            // Restore identity annotations to old primary key columns
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.supplier ALTER COLUMN suplier_code ADD GENERATED ALWAYS AS IDENTITY;
                ALTER TABLE kwarehouse.stores ALTER COLUMN store_code ADD GENERATED ALWAYS AS IDENTITY;
            ");

            migrationBuilder.AddPrimaryKey(
                name: "PK_supplier",
                schema: "kwarehouse",
                table: "supplier",
                column: "suplier_code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stores",
                schema: "kwarehouse",
                table: "stores",
                column: "store_code");

            migrationBuilder.AddForeignKey(
                name: "FK_h_inv_trans_supplier_suplier_code",
                schema: "kwarehouse",
                table: "h_inv_trans",
                column: "suplier_code",
                principalSchema: "kwarehouse",
                principalTable: "supplier",
                principalColumn: "suplier_code");
        }
    }
}
