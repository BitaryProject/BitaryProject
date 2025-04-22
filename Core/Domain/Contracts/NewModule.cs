/*
using Domain.Entities.AppointmentEntities;
using Domain.Entities.ClinicEntities;
using Domain.Entities.DoctorEntites;
using Domain.Entities.MedicalRecordEntites;
using Domain.Entities.PetEntities;
using Domain.Contracts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.NewModule
{
   
    public interface IPetRepository : IGenericRepository<Pet, int>
    {
        Task<IEnumerable<Pet>> GetPetsByUserIdAsync(string userId);
        Task<(IEnumerable<Pet> Pets, int TotalCount)> GetPagedPetsAsync(Specifications<Pet> specifications, int pageIndex, int pageSize);
    }

    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord, int>
    {
        Task<IEnumerable<MedicalRecord>> GetRecordsByPetIdAsync(int petId);
        Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> GetPagedMedicalRecordsAsync(Specifications<MedicalRecord> specifications, int pageIndex, int pageSize);
    }

    public interface IDoctorRepository : IGenericRepository<Doctor, int>
    {
        Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(Specifications<Doctor> specifications, int pageIndex, int pageSize);
    }

    public interface IClinicRepository : IGenericRepository<Clinic, int>
    {
        Task<IEnumerable<Clinic>> GetClinicsByCityAsync(string city);
        Task<IEnumerable<Clinic>> GetClinicsByNameAsync(string clinicName);
        Task<IEnumerable<Clinic>> GetTopRatedClinicsAsync(int count);
        Task<IEnumerable<Clinic>> GetClinicsByStatusAsync(ClinicStatus status);
        Task<IEnumerable<Clinic>> GetClinicsByOwnerIdAsync(string ownerId);
        Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetPagedClinicsAsync(Specifications<Clinic> specifications, int pageIndex, int pageSize);
        Task<IEnumerable<Clinic>> SearchClinicsInRadiusAsync(string city, int radiusKm);
    }

    public interface IAppointmentRepository : IGenericRepository<Appointment, int>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPetIdAsync(int petId);
        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetPagedAppointmentsAsync(Specifications<Appointment> specifications, int pageIndex, int pageSize);
    }

    public interface INewModuleUnitOfWork : IUnitOFWork
    {
        Task<int> SaveChangesAsync();
        IPetRepository PetRepository { get; }
        IMedicalRecordRepository MedicalRecordRepository { get; }
        IDoctorRepository DoctorRepository { get; }
        IClinicRepository ClinicRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
    }
}
*/
