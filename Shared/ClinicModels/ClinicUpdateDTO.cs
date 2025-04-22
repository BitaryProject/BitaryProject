using Domain.Entities.ClinicEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ClinicModels
{
    public record ClinicUpdateDTO
    {
        public string ClinicName { get; set; }
        public ClinicAddress Address { get; set; }
    }
} 