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
        public Guid Id { get; init; }
        public string ClinicName { get; init; }
        public Address Address { get; init; }
        public decimal ExaminationFee { get; init; }
    }
}
