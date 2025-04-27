using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Persistence.Migrations
{
    public partial class UpdateDoctorScheduleToUseDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First drop the existing unique index
            migrationBuilder.DropIndex(
                name: "IX_DoctorSchedules_DoctorId_Day",
                table: "DoctorSchedules");
                
            // Rename Day column to ScheduleDate and change its type to DateTime
            migrationBuilder.DropColumn(
                name: "Day",
                table: "DoctorSchedules");
                
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDate",
                table: "DoctorSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            
            // Add new indexes
            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId",
                table: "DoctorSchedules",
                column: "DoctorId");
                
            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_ScheduleDate",
                table: "DoctorSchedules",
                column: "ScheduleDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new indexes
            migrationBuilder.DropIndex(
                name: "IX_DoctorSchedules_DoctorId",
                table: "DoctorSchedules");
                
            migrationBuilder.DropIndex(
                name: "IX_DoctorSchedules_ScheduleDate",
                table: "DoctorSchedules");
                
            // Revert the column change
            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "DoctorSchedules");
                
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "DoctorSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
                
            // Recreate the original index
            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId_Day",
                table: "DoctorSchedules",
                columns: new[] { "DoctorId", "Day" },
                unique: true);
        }
    }
} 