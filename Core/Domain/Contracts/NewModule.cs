/*using Domain.Entities.AppointmentEntities;
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
   
        public interface IPetRepository : IGenericRepository<Pet, Guid>
        {
            Task<IEnumerable<Pet>> GetPetsByUserIdAsync(string userId);
            Task<(IEnumerable<Pet> Pets, int TotalCount)> GetPagedPetsAsync(Specifications<Pet> specifications, int pageIndex, int pageSize);
        }

        public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord, Guid>
        {
            Task<IEnumerable<MedicalRecord>> GetRecordsByPetIdAsync(Guid petId);
            Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> GetPagedMedicalRecordsAsync(Specifications<MedicalRecord> specifications, int pageIndex, int pageSize);
        }

        public interface IDoctorRepository : IGenericRepository<Doctor, Guid>
        {
            Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyAsync(string specialty);
            Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(Specifications<Doctor> specifications, int pageIndex, int pageSize);
        }

        public interface IClinicRepository : IGenericRepository<Clinic, Guid>
        {
            Task<IEnumerable<Clinic>> GetClinicsByCityAsync(string city);
            Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetPagedClinicsAsync(Specifications<Clinic> specifications, int pageIndex, int pageSize);

            Task<IEnumerable<Clinic>> GetTopRatedClinicsAsync(int count);
            Task<IEnumerable<Clinic>> SearchClinicsInRadiusAsync(string city, int radiusKm);

        }

    public interface IAppointmentRepository : IGenericRepository<Appointment, Guid>
        {
            Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId);
            Task<IEnumerable<Appointment>> GetAppointmentsByPetIdAsync(Guid petId);

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