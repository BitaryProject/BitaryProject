using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations.Identity
{
    public partial class AddUserRoleProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the column exists first
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'UserRole'
                )
                BEGIN
                    -- Add the column as nullable first
                    ALTER TABLE AspNetUsers
                    ADD UserRole TINYINT NULL;
                END
            ");

            // Set default values for existing records
            migrationBuilder.Sql(@"
                UPDATE AspNetUsers
                SET UserRole = 1
                WHERE Discriminator = 'User' AND (UserRole IS NULL OR UserRole = 0);
            ");

            // Try to update the column to be not null if possible
            // Only for rows where Discriminator = 'User'
            migrationBuilder.Sql(@"
                -- Rather than altering the column to NOT NULL, 
                -- we'll ensure that the application sets the value correctly
                -- For the User entity, we'll ensure it has the right discriminator

                -- Add a default constraint if one doesn't exist
                IF NOT EXISTS (
                    SELECT * FROM sys.default_constraints 
                    WHERE parent_object_id = OBJECT_ID('AspNetUsers')
                    AND COL_NAME(parent_object_id, parent_column_id) = 'UserRole'
                )
                BEGIN
                    ALTER TABLE AspNetUsers
                    ADD CONSTRAINT DF_AspNetUsers_UserRole DEFAULT 1 FOR UserRole;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // We don't provide a down migration as it could lead to data loss
        }
    }
} 