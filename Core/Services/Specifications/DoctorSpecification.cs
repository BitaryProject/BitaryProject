//using Domain.Entities.DoctorEntites;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Services.Specifications
//{
//    public class DoctorSpecification : Specifications<Doctor>
//    {
//        public DoctorSpecification(int id)
//            : base(d => d.Id == id)
//        {
//            AddInclude(d => d.Clinic);
//        }

       
//        public DoctorSpecification(string specialty, int pageIndex, int pageSize)
//            : base(d => d.Specialty.ToLower().Contains(specialty.ToLower()))
//        {
//            ApplyPagination(pageIndex, pageSize);
//            setOrderBy(d => d.Name);
//        }
//    }
//}
