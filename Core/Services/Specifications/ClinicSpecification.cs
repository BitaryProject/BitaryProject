using Domain.Entities.ClinicEntities;
using System;
using System.Globalization;

namespace Services.Specifications
{
    public class ClinicSpecification : Specifications<Clinic>
    {
        public ClinicSpecification(Guid id)
            : base(c => c.Id == id)
        {
        }

        public ClinicSpecification(string city, int pageIndex, int pageSize, string? clinicName = null)
            : base(c => c.Address.City.ToLowerInvariant().Contains(city.ToLowerInvariant()) &&
                        (string.IsNullOrWhiteSpace(clinicName) || c.ClinicName.ToLowerInvariant().Contains(clinicName.ToLowerInvariant())))
        {
            ApplyPagination(pageIndex, pageSize);
            setOrderBy(c => c.ClinicName);
        }
    }
}

