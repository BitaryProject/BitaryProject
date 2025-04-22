using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Persistence.Migrations
{
    public partial class AddClinicsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ClinicName = table.Column<string>(maxLength: 100, nullable: false),
                    Rating = table.Column<double>(nullable: false, defaultValue: 0.0),
                    Status = table.Column<int>(nullable: false, defaultValue: 0),
                    OwnerId = table.Column<string>(nullable: false),
                    Address_Name = table.Column<string>(maxLength: 100, nullable: true),
                    Address_Street = table.Column<string>(maxLength: 200, nullable: true),
                    Address_City = table.Column<string>(maxLength: 100, nullable: true),
                    Address_Country = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.Id);
                    // Remove the foreign key constraint here
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_OwnerId",
                table: "Clinics",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clinics");
        }
    }
}
    