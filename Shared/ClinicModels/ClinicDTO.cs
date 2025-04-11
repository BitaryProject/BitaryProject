using Domain.Entities.ClinicEntities;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ClinicModels
{
    public record ClinicDTO
    {
        public Guid Id { get; set; }
        public string ClinicName { get; set; }
        public ClinicAddress Address { get; set; }

        public decimal ExaminationFee { get; set; } // Ensure this is decimal

        public double Rating { get; set; }
    }

}
