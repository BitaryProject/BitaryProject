using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Persistence.Migrations
{
    public partial class AddRatingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add RatingCount column to Clinics table
            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Clinics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Create Ratings table
            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ClinicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index on ClinicId column for faster lookups
            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ClinicId",
                table: "Ratings",
                column: "ClinicId");
                
            // Create index on UserId column for faster lookups by user
            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");
                
            // Create composite index for faster lookups of user ratings for a specific clinic
            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId_ClinicId",
                table: "Ratings",
                columns: new[] { "UserId", "ClinicId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop Ratings table
            migrationBuilder.DropTable(
                name: "Ratings");

            // Remove RatingCount column from Clinics table
            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Clinics");
        }
    }
} 