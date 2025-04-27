using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.DoctorModels
{
    public record DoctorScheduleDTO
    {
        public int Id { get; init; }
        public int DoctorId { get; init; }
        
        [Required]
        public DateTime ScheduleDate { get; init; }
        
        [Required]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Start time must be in format HH:MM (24-hour format)")]
        public string StartTimeString { get; init; }
        
        [Required]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "End time must be in format HH:MM (24-hour format)")]
        public string EndTimeString { get; init; }
        
        [JsonIgnore]
        public TimeSpan StartTime => TimeSpan.Parse(StartTimeString ?? "00:00");
        
        [JsonIgnore]
        public TimeSpan EndTime => TimeSpan.Parse(EndTimeString ?? "00:00");
        
        public string? DoctorName { get; init; }
    }
}
