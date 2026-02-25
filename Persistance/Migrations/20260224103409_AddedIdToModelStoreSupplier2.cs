using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdToModelStoreSupplier2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_supplier_suplier_code",
                schema: "kwarehouse",
                table: "supplier",
                column: "suplier_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stores_store_code",
                schema: "kwarehouse",
                table: "stores",
                column: "store_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_supplier_suplier_code",
                schema: "kwarehouse",
                table: "supplier");

            migrationBuilder.DropIndex(
                name: "IX_stores_store_code",
                schema: "kwarehouse",
                table: "stores");
        }
    }
}
