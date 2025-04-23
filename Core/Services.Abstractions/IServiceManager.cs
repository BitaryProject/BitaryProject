using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IServiceManager
    {
        public IProductService ProductService { get; }
        public IBasketService BasketService { get; }
        public IOrderService OrderService { get; }
        public IAuthenticationService AuthenticationService { get; }
        public IPaymentService PaymentService { get; }
        public IPetService PetService { get; }
        public IDoctorService DoctorService { get; }
        public IClinicService ClinicService { get; }
        public IDoctorScheduleService DoctorScheduleService { get; }
        //public IAppointmentService AppointmentService { get; }
        //public IMedicalRecordService MedicalRecordService { get; }
        //IClinicSearchService ClinicSearchService { get; }
    }
}
