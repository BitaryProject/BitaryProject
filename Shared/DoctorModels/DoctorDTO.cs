using System;

namespace Shared.DoctorModels
{
    public record DoctorDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Specialty { get; init; }
        public string Email { get; init; }
        public string Phone { get; init; }
        public string Gender { get; init; }
        public Guid ClinicId { get; init; }
        public string ImageUrl { get; init; }
        public string Description { get; init; }
        public double Rating { get; init; }
        public bool IsAvailable { get; init; }
    }
}
