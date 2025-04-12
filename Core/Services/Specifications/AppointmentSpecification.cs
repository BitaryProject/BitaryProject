//using Domain.Entities.AppointmentEntities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Services.Specifications
//{
//    public class AppointmentSpecification : Specifications<Appointment>
//    {

//        public AppointmentSpecification(int id)
//            : base(a => a.Id == id)
//        {
//            AddInclude(a => a.PetProfile);
//            AddInclude(a => a.Clinic);
//            AddInclude(a => a.Doctor);
//        }

//        public AppointmentSpecification(string userId, int pageIndex, int pageSize)
//            : base(a => a.UserId == userId)
//        {
//            ApplyPagination(pageIndex, pageSize);
//            setOrderByDescending(a => a.AppointmentDate);
//        }

//        public AppointmentSpecification(int petId, int pageIndex, int pageSize)
//            : base(a => a.PetId == petId)
//        {
//            ApplyPagination(pageIndex, pageSize);
//            setOrderByDescending(a => a.AppointmentDate);
//        }
//    }
//}
