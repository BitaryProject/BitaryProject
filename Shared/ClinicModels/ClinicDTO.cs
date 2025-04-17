using System;

namespace Shared.ClinicModels
{
    public record ClinicDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Address { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public string Description { get; init; }
        public string Website { get; init; }
        public string ImageUrl { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public double Rating { get; init; }
    }
}
