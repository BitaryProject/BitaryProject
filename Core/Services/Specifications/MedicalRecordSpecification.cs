﻿//using Domain.Entities.MedicalRecordEntites;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Services.Specifications
//{
//    public class MedicalRecordSpecification : Specifications<MedicalRecord>
//    {
//        public MedicalRecordSpecification(int id)
//            : base(r => r.Id == id)
//        {
//            AddInclude(r => r.Pet);
//            AddInclude(r => r.Doctor);
//        }

//        public MedicalRecordSpecification(int petId, int pageIndex, int pageSize, DateTime? start = null, DateTime? end = null)
//            : base(r => r.PetId == petId &&
//                        (!start.HasValue || r.RecordDate >= start.Value) &&
//                        (!end.HasValue || r.RecordDate <= end.Value))
//        {
//            ApplyPagination(pageIndex, pageSize);
//            setOrderByDescending(r => r.RecordDate);
//        }
//    }
//}
