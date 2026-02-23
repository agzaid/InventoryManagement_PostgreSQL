using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "branchdescarabic",
            //    schema: "kwarehouse",
            //    table: "branches");

            //migrationBuilder.AlterColumn<string>(
            //    name: "branch_code",
            //    schema: "kwarehouse",
            //    table: "branches",
            //    type: "character(1)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "integer")
            //    .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "branch_code",
                schema: "kwarehouse",
                table: "branches",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(1)")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "branchdescarabic",
                schema: "kwarehouse",
                table: "branches",
                type: "text",
                nullable: true);
        }
    }
}
