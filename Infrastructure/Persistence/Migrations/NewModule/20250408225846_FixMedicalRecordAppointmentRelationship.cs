using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations.NewModule
{
    /// <inheritdoc />
    public partial class FixMedicalRecordAppointmentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Appointments_AppointmentId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "MedicalRecordId",
                table: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Appointments_AppointmentId",
                table: "MedicalRecords",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Appointments_AppointmentId",
                table: "MedicalRecords");

            migrationBuilder.AddColumn<Guid>(
                name: "MedicalRecordId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Appointments_AppointmentId",
                table: "MedicalRecords",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
