using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class MakeRoleIdNullableInRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint first
            migrationBuilder.DropForeignKey(
                name: "FK_rolepermissions_aspnetroles_roleid",
                table: "rolepermissions");

            // Alter the column to be nullable
            migrationBuilder.AlterColumn<string>(
                name: "roleid",
                table: "rolepermissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // Re-add the foreign key constraint with nullable support
            migrationBuilder.AddForeignKey(
                name: "FK_rolepermissions_aspnetroles_roleid",
                table: "rolepermissions",
                column: "roleid",
                principalTable: "aspnetroles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_rolepermissions_aspnetroles_roleid",
                table: "rolepermissions");

            // Alter the column back to non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "roleid",
                table: "rolepermissions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Re-add the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_rolepermissions_aspnetroles_roleid",
                table: "rolepermissions",
                column: "roleid",
                principalTable: "aspnetroles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
