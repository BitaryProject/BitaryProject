//using Shared.DoctorModels;
//using Domain.Entities.DoctorEntites;
//using Domain.Exceptions;
//using System;
//using System.Threading.Tasks;

//namespace Services
//{
//    public class DoctorScheduleService : IDoctorScheduleService
//    {
//        private readonly IDoctorScheduleRepository _repository;
//        private readonly IMapper _mapper;

//        public DoctorScheduleService(
//            IDoctorScheduleRepository repository,
//            IMapper mapper)
//        {
//            _repository = repository;
//            _mapper = mapper;
//        }

//        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime date)
//        {
//            var utcDate = date.ToUniversalTime();

//            var schedule = await _repository.GetScheduleAsync(doctorId, utcDate.DayOfWeek);

//            if (schedule == null)
//                throw new ScheduleNotFoundException(doctorId, utcDate.DayOfWeek);

//            var time = utcDate.TimeOfDay;
//            return time >= schedule.StartTime && time <= schedule.EndTime;
//        }

//        public async Task AddScheduleAsync(int doctorId, DoctorScheduleDTO dto)
//        {
//            var existing = await _repository.GetScheduleAsync(doctorId, dto.Day);
//            if (existing != null)
//                throw new ScheduleConflictException("جدول موجود بالفعل لهذا اليوم");

//            var schedule = _mapper.Map<DoctorSchedule>(dto);
//            schedule.DoctorId = doctorId;

//            if (schedule.StartTime >= schedule.EndTime)
//                throw new InvalidScheduleException("وقت البدء يجب أن يكون قبل وقت الانتهاء");

//            await _repository.AddAsync(schedule);
//            await _repository.SaveChangesAsync();
//        }
//    }
//}