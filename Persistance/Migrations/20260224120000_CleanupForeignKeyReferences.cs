using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class CleanupForeignKeyReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up invalid foreign key references before adding constraints
            migrationBuilder.Sql(@"
                UPDATE kwarehouse.h_inv_trans t
                SET suplier_code = NULL
                WHERE suplier_code IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM kwarehouse.supplier s
                      WHERE s.suplier_code = t.suplier_code
                  );
            ");

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

            migrationBuilder.Sql(@"
                UPDATE kwarehouse.h_inv_trans t
                SET store_code = NULL
                WHERE store_code IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM kwarehouse.stores s
                      WHERE s.store_code = t.store_code
                  );
            ");

            migrationBuilder.Sql(@"
                UPDATE kwarehouse.inv_trans t
                SET storecode = NULL
                WHERE storecode IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM kwarehouse.stores s
                      WHERE s.store_code = t.storecode
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No cleanup needed for rollback
        }
    }
}
