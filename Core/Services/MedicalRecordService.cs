using AutoMapper;
using Domain.Entities.MedicalRecordEntites;
using Domain.Contracts;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared.MedicalRecordModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.AppointmentEntities;

namespace Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointmentService;

        public MedicalRecordService(IUnitOFWork unitOfWork, IMapper mapper, IAppointmentService appointmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentService = appointmentService;
        }

        public async Task<MedicalRecordDTO> GetMedicalRecordByIdAsync(int recordId)
        {
            var spec = new MedicalRecordSpecification(recordId);
            var record = await _unitOfWork.GetRepository<MedicalRecord, int>().GetAsync(spec);

            if (record == null)
                throw new MedicalRecordNotFoundException(recordId.ToString());

            return _mapper.Map<MedicalRecordDTO>(record);
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetRecordsByPetIdAsync(int petId)
        {
            var spec = new MedicalRecordSpecification(petId, true);
            var records = await _unitOfWork.GetRepository<MedicalRecord, int>().GetAllAsync(spec);
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetRecordsByDoctorIdAsync(int doctorId)
        {
            var spec = MedicalRecordSpecification.GetByDoctorId(doctorId);
            var records = await _unitOfWork.GetRepository<MedicalRecord, int>().GetAllAsync(spec);
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<MedicalRecordDTO> GetRecordByAppointmentIdAsync(int appointmentId)
        {
            var spec = MedicalRecordSpecification.GetByAppointmentId(appointmentId);
            var record = await _unitOfWork.GetRepository<MedicalRecord, int>().GetAsync(spec);

            if (record == null)
                throw new MedicalRecordNotFoundException($"No medical record found for appointment ID: {appointmentId}");

            return _mapper.Map<MedicalRecordDTO>(record);
        }

        public async Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordDTO model)
        {
            var record = _mapper.Map<MedicalRecord>(model);

            // Set record date to now if not provided
            if (record.RecordDate == default)
            {
                record.RecordDate = DateTime.UtcNow;
            }

            await _unitOfWork.GetRepository<MedicalRecord, int>().AddAsync(record);
            await _unitOfWork.SaveChangesAsync();

            // Return the created record with relationships
            return await GetMedicalRecordByIdAsync(record.Id);
        }

        public async Task<MedicalRecordDTO> UpdateMedicalRecordAsync(int recordId, MedicalRecordDTO model)
        {
            var spec = new MedicalRecordSpecification(recordId);
            var record = await _unitOfWork.GetRepository<MedicalRecord, int>().GetAsync(spec);

            if (record == null)
                throw new MedicalRecordNotFoundException(recordId.ToString());

            // Only update certain fields
            record.Diagnosis = model.Diagnosis;
            record.Treatment = model.Treatment;
            record.Notes = model.Notes;

            _unitOfWork.GetRepository<MedicalRecord, int>().Update(record);
            await _unitOfWork.SaveChangesAsync();

            // Return the updated record with relationships
            return await GetMedicalRecordByIdAsync(recordId);
        }

        public async Task<bool> DeleteMedicalRecordAsync(int recordId)
        {
            var record = await _unitOfWork.GetRepository<MedicalRecord, int>().GetAsync(recordId);

            if (record == null)
                throw new MedicalRecordNotFoundException(recordId.ToString());

            _unitOfWork.GetRepository<MedicalRecord, int>().Delete(record);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<MedicalRecordDTO> UpdateMedicalRecordPartialAsync(int recordId, MedicalRecordUpdateDTO model)
        {
            var spec = new MedicalRecordSpecification(recordId);
            var record = await _unitOfWork.GetRepository<MedicalRecord, int>().GetAsync(spec);

            if (record == null)
                throw new MedicalRecordNotFoundException(recordId.ToString());

            // Only update the fields from the UpdateDTO
            record.Diagnosis = model.Diagnosis;
            record.Treatment = model.Treatment;
            record.Notes = model.Notes;

            _unitOfWork.GetRepository<MedicalRecord, int>().Update(record);
            await _unitOfWork.SaveChangesAsync();

            // Return the updated record with relationships
            return await GetMedicalRecordByIdAsync(recordId);
        }

        public async Task<MedicalRecordDTO> CreateMedicalRecordForAppointmentAsync(int appointmentId, MedicalRecordCreateDTO model, int doctorId)
        {
            // Get the appointment
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

            // Check if the appointment belongs to the doctor
            if (appointment.DoctorId != doctorId)
            {
                throw new InvalidOperationException("You can only create medical records for your own appointments.");
            }

            // Check if the appointment is completed
            if (appointment.Status != AppointmentStatus.Completed)
            {
                // Update the appointment status to Completed
                var updatedAppointment = appointment with { Status = AppointmentStatus.Completed };
                await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, updatedAppointment);
            }
            try
            {
                var existingRecord = await GetRecordByAppointmentIdAsync(appointmentId);
                if (existingRecord != null)
                {
                    throw new InvalidOperationException($"A medical record already exists for appointment ID: {appointmentId}");
                }
            }
            catch (MedicalRecordNotFoundException)
            {
                // This is the expected case - no record exists
            }

            // Create a new medical record
            var record = _mapper.Map<MedicalRecord>(model);
            record.PetId = appointment.PetId;
            record.DoctorId = doctorId;
            record.AppointmentId = appointmentId;

            // Set record date to now if not provided
            if (record.RecordDate == default)
            {
                record.RecordDate = DateTime.UtcNow;
            }

            await _unitOfWork.GetRepository<MedicalRecord, int>().AddAsync(record);
            await _unitOfWork.SaveChangesAsync();

            // Return the created record with relationships
            return await GetMedicalRecordByIdAsync(record.Id);
        }

        // Check if a record already exists for this appointment
    }
    }
