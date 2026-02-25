using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class changedTypeOfCategroyCodeINItemAndItemCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, handle data conversion for item_category table
            migrationBuilder.Sql(@"
                -- Add temporary integer column
                ALTER TABLE kwarehouse.item_category ADD COLUMN catgry_code_temp integer;
                
                -- Convert string codes to integers (handle NULL values and invalid data)
                UPDATE kwarehouse.item_category 
                SET catgry_code_temp = CASE 
                    WHEN catgry_code ~ '^[0-9]+$' THEN CAST(catgry_code AS integer)
                    WHEN catgry_code IS NULL THEN id
                    ELSE id
                END;
                
                -- Drop the old column
                ALTER TABLE kwarehouse.item_category DROP COLUMN catgry_code;
                
                -- Rename the temporary column
                ALTER TABLE kwarehouse.item_category RENAME COLUMN catgry_code_temp TO catgry_code;
                
                -- Make it NOT NULL
                ALTER TABLE kwarehouse.item_category ALTER COLUMN catgry_code SET NOT NULL;
            ");

            // Then, handle data conversion for items table
            migrationBuilder.Sql(@"
                -- Add temporary integer column
                ALTER TABLE kwarehouse.items ADD COLUMN catgry_code_temp integer;
                
                -- Convert string codes to integers (handle NULL values and invalid data)
                UPDATE kwarehouse.items 
                SET catgry_code_temp = CASE 
                    WHEN catgry_code ~ '^[0-9]+$' THEN CAST(catgry_code AS integer)
                    ELSE 0
                END;
                
                -- Drop the old column
                ALTER TABLE kwarehouse.items DROP COLUMN catgry_code;
                
                -- Rename the temporary column
                ALTER TABLE kwarehouse.items RENAME COLUMN catgry_code_temp TO catgry_code;
                
                -- Make it NOT NULL
                ALTER TABLE kwarehouse.items ALTER COLUMN catgry_code SET NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Handle reverse migration for item_category table
            migrationBuilder.Sql(@"
                -- Add temporary string column
                ALTER TABLE kwarehouse.item_category ADD COLUMN catgry_code_temp text;
                
                -- Convert integer codes to strings with padding
                UPDATE kwarehouse.item_category 
                SET catgry_code_temp = LPAD(CAST(catgry_code AS text), 2, '0');
                
                -- Drop the old column
                ALTER TABLE kwarehouse.item_category DROP COLUMN catgry_code;
                
                -- Rename the temporary column
                ALTER TABLE kwarehouse.item_category RENAME COLUMN catgry_code_temp TO catgry_code;
            ");

            // Handle reverse migration for items table
            migrationBuilder.Sql(@"
                -- Add temporary string column
                ALTER TABLE kwarehouse.items ADD COLUMN catgry_code_temp character varying(2);
                
                -- Convert integer codes to strings with padding
                UPDATE kwarehouse.items 
                SET catgry_code_temp = LPAD(CAST(catgry_code AS text), 2, '0');
                
                -- Drop the old column
                ALTER TABLE kwarehouse.items DROP COLUMN catgry_code;
                
                -- Rename the temporary column
                ALTER TABLE kwarehouse.items RENAME COLUMN catgry_code_temp TO catgry_code;
                
                -- Make it nullable
                ALTER TABLE kwarehouse.items ALTER COLUMN catgry_code DROP NOT NULL;
            ");
        }
    }
}
