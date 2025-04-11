using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.AppointmentModels
{
    public record AppointmentDTO
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public Guid PetId { get; init; }
        public Guid ClinicId { get; init; }
        public Guid? DoctorId { get; init; }
        public DateTime AppointmentDate { get; init; }
        public string Status { get; init; }
        public string Notes { get; init; }
    }
}
