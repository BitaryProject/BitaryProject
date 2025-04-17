using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;
using Core.Domain.Entities.HealthcareEntities;

namespace Core.Services.Specifications
{
    public class MedicationSpecification : BaseSpecification<Medication>
    {
        public MedicationSpecification(Guid id) 
            : base(m => m.Id == id)
        {
        }

        public MedicationSpecification(string name, int pageIndex, int pageSize)
            : base(m => string.IsNullOrEmpty(name) || m.Name.ToLower().Contains(name.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(m => m.Name);
        }

        public MedicationSpecification(string dosageForm, bool searchByDosageForm, int pageIndex, int pageSize)
            : base(m => m.DosageForm.ToLower().Contains(dosageForm.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(m => m.Name);
        }

        public MedicationSpecification(string manufacturer, string searchByManufacturerParam, int pageIndex, int pageSize)
            : base(m => m.Manufacturer.ToLower().Contains(manufacturer.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(m => m.Name);
        }

        public MedicationSpecification(decimal minPrice, decimal maxPrice, int pageIndex, int pageSize)
            : base(m => m.Price >= minPrice && m.Price <= maxPrice)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(m => m.Price);
        }

        public MedicationSpecification(Expression<Func<Medication, bool>> criteria)
            : base(criteria)
        {
        }

        public MedicationSpecification()
            : base(null)
        {
            AddOrderBy(m => m.Name);
        }
    }
} 
