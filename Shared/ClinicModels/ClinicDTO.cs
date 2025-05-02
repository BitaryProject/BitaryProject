using Domain.Entities.ClinicEntities;
using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ClinicModels
{
    public record ClinicDTO
    {
        public int Id { get; set; }
        public string ClinicName { get; set; }
        public ClinicAddress Address { get; set; }
        public double Rating { get; set; }
        public ClinicStatus Status { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public List<DoctorDTO> Doctors { get; set; } = new List<DoctorDTO>();
    }
}
