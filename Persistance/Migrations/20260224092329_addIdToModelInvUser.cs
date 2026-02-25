using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addIdToModelInvUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.inv_users DROP CONSTRAINT IF EXISTS ""PK_inv_users"";
                ALTER TABLE kwarehouse.inv_users DROP CONSTRAINT IF EXISTS inv_users_pkey;
            ");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "kwarehouse",
                table: "inv_users",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_inv_users",
                schema: "kwarehouse",
                table: "inv_users",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_inv_users_user_code",
                schema: "kwarehouse",
                table: "inv_users",
                column: "user_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE kwarehouse.inv_users DROP CONSTRAINT IF EXISTS ""PK_inv_users"";
                ALTER TABLE kwarehouse.inv_users DROP CONSTRAINT IF EXISTS inv_users_pkey;
            ");

            migrationBuilder.DropIndex(
                name: "IX_inv_users_user_code",
                schema: "kwarehouse",
                table: "inv_users");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "kwarehouse",
                table: "inv_users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_inv_users",
                schema: "kwarehouse",
                table: "inv_users",
                column: "user_code");
        }
    }
}
