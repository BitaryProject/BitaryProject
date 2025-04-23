using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DoctorModels
{
    public record DoctorCreateModel
    {
        [Required(ErrorMessage = "Specialty is required")]
        public string Specialty { get; init; }
        
        [Required(ErrorMessage = "Clinic ID is required")]
        public int ClinicId { get; init; }
    }
} 