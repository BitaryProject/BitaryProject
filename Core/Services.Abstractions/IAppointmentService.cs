//using Shared.AppointmentModels;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Services.Abstractions
//{
//    public interface IAppointmentService
//    {
//        Task<AppointmentDTO?> GetAppointmentByIdAsync(int appointmentId);
//        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByUserIdAsync(string userId);
//        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPetIdAsync(int petId);

//        Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO model);
//        Task<AppointmentDTO?> UpdateAppointmentAsync(int appointmentId, AppointmentDTO model);

//        Task<bool> DeleteAppointmentAsync(int appointmentId);
//    }
//}
