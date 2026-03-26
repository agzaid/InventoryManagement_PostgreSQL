using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class lowstock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "minimumquantity",
                schema: "kwarehouse",
                table: "items",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "notificationpercentage",
                schema: "kwarehouse",
                table: "items",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "minimumquantity",
                schema: "kwarehouse",
                table: "items");

            migrationBuilder.DropColumn(
                name: "notificationpercentage",
                schema: "kwarehouse",
                table: "items");
        }
    }
}
