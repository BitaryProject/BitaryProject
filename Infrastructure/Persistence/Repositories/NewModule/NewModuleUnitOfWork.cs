using Core.Common.Specifications;

//using Core.Domain.Contracts.NewModule;
//using Infrastructure.Persistence.Data;
//using System;
//using System.Collections.Concurrent;
//using System.Threading.Tasks;
//using Core.Domain.Entities;

//namespace Infrastructure.Persistence.Repositories.NewModule
//{
//    public class NewModuleUnitOfWork : INewModuleUnitOfWork
//    {
//        private readonly NewModuleContext _context;
//        private readonly ConcurrentDictionary<string, object> _repositories;

//        public NewModuleUnitOfWork(NewModuleContext context)
//        {
//            _context = context;
//            _repositories = new ConcurrentDictionary<string, object>();
//        }

//        public async Task<int> SaveChangesAsync()
//            => await _context.SaveChangesAsync();

//        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
//        {
//            return (IGenericRepository<TEntity, TKey>)_repositories.GetOrAdd(typeof(TEntity).Name, _ => new NewModuleGenericRepository<TEntity, TKey>(_context));
//        }

//        public IPetRepository PetRepository => GetOrCreateRepository<IPetRepository, PetRepository>();
//        public IMedicalRecordRepository MedicalRecordRepository => GetOrCreateRepository<IMedicalRecordRepository, MedicalRecordRepository>();
//        public IDoctorRepository DoctorRepository => GetOrCreateRepository<IDoctorRepository, DoctorRepository>();
//        public IClinicRepository ClinicRepository => GetOrCreateRepository<IClinicRepository, ClinicRepository>();
//        public IAppointmentRepository AppointmentRepository => GetOrCreateRepository<IAppointmentRepository, AppointmentRepository>();
//        public IDoctorScheduleRepository DoctorScheduleRepository => GetOrCreateRepository<IDoctorScheduleRepository, DoctorScheduleRepository>();

//        private TInterface GetOrCreateRepository<TInterface, TImplementation>()
//            where TImplementation : TInterface
//        {
//            return (TInterface)_repositories.GetOrAdd(typeof(TInterface).Name, _ => Activator.CreateInstance(typeof(TImplementation), _context)!);
//        }
//    }
//}








