-- Migration: Make RoleId nullable in RolePermissions table

-- Drop the foreign key constraint first
ALTER TABLE rolepermissions DROP CONSTRAINT IF EXISTS "FK_rolepermissions_aspnetroles_roleid";

-- Alter the column to be nullable
ALTER TABLE rolepermissions ALTER COLUMN roleid DROP NOT NULL;

-- Re-add the foreign key constraint with nullable support
ALTER TABLE rolepermissions 
ADD CONSTRAINT "FK_rolepermissions_aspnetroles_roleid" 
FOREIGN KEY (roleid) 
REFERENCES aspnetroles(id) 
ON DELETE CASCADE;

-- Insert migration history record
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260326131900_MakeRoleIdNullableInRolePermissions', '8.0.1');
