using Domain.Entities.ClinicEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ClinicModels
{
    public record ClinicStatusUpdateDTO
    {
        public ClinicStatus Status { get; set; }
    }
} 