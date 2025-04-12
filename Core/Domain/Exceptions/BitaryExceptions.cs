//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Domain.Exceptions
//{
//    public class PetNotFoundException : Exception
//    {
//        public PetNotFoundException()
//            : base("The requested pet was not found.") { }

//        public PetNotFoundException(string petId)
//            : base($"No pet found with id: {petId}.") { }
//    }

//    public class MedicalRecordNotFoundException : Exception
//    {
//        public MedicalRecordNotFoundException()
//            : base("The requested medical record was not found.") { }

//        public MedicalRecordNotFoundException(string recordId)
//            : base($"No medical record found with id: {recordId}.") { }
//    }

//    public class DoctorNotFoundException : Exception
//    {
//        public DoctorNotFoundException()
//            : base("The requested doctor was not found.") { }

//        public DoctorNotFoundException(string doctorId)
//            : base($"No doctor found with id: {doctorId}.") { }
//    }

//    public class ClinicNotFoundException : Exception
//    {
//        public ClinicNotFoundException()
//            : base("The requested clinic was not found.") { }

//        public ClinicNotFoundException(string clinicId)
//            : base($"No clinic found with id: {clinicId}.") { }
//    }

//    public class AppointmentNotFoundException : Exception
//    {
//        public AppointmentNotFoundException()
//            : base("The requested appointment was not found.") { }

//        public AppointmentNotFoundException(string appointmentId)
//            : base($"No appointment found with id: {appointmentId}.") { }
//    }



//    public class ScheduleNotFoundException : Exception
//    {
//        public ScheduleNotFoundException(int doctorId, DayOfWeek day)
//            : base($"No schedule found for doctor {doctorId} on {day}") { }
//    }

//    public class ScheduleConflictException : Exception
//    {
//        public ScheduleConflictException(string message) : base(message) { }
//    }

//    public class InvalidScheduleException : Exception
//    {
//        public InvalidScheduleException(string message) : base(message) { }
//    }
//}
