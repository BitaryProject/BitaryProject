using AutoMapper;
using Core.Services.Abstractions;
using Core.Domain.Contracts;
using Microsoft.Extensions.Logging;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Services.Specifications;
using Shared;
using Core.Services.Specifications.Base;

namespace Core.Services
{
    public class RatingService : IRatingService
    {
        private readonly IHealthcareUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RatingService> _logger;

        public RatingService(
            IHealthcareUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RatingService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Doctor Ratings

        public async Task<DoctorRatingDTO> GetDoctorRatingByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting doctor rating with ID: {Id}", id);
            
            var specification = new BaseSpecification<Core.Domain.Entities.HealthcareEntities.DoctorRating>(r => r.Id == id);
            specification.AddInclude(r => r.Doctor);
            specification.AddInclude(r => r.PetOwner);
            
            var rating = await _unitOfWork.DoctorRatingRepository.GetAsync(specification);
            
            if (rating == null)
            {
                _logger.LogWarning("Doctor rating with ID {Id} not found", id);
                return null;
            }
            
            return _mapper.Map<DoctorRatingDTO>(rating);
        }

        public async Task<IEnumerable<DoctorRatingDTO>> GetDoctorRatingsByDoctorIdAsync(Guid doctorId)
        {
            _logger.LogInformation("Getting doctor ratings for doctor ID: {DoctorId}", doctorId);
            
            var specification = new DoctorRatingByDoctorIdSpecification1(doctorId);
            var ratings = await _unitOfWork.DoctorRatingRepository.GetAllAsync(specification);
            
            return _mapper.Map<IEnumerable<DoctorRatingDTO>>(ratings);
        }

        public async Task<PaginatedResult<DoctorRatingDTO>> GetPagedDoctorRatingsAsync(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Getting paged doctor ratings. Page: {PageIndex}, Size: {PageSize}", pageIndex, pageSize);
            
            var specification = new DoctorRatingPaginatedSpecification1(pageIndex, pageSize);
            var countSpec = new BaseSpecification<Core.Domain.Entities.HealthcareEntities.DoctorRating>();
            
            var totalItems = await _unitOfWork.DoctorRatingRepository.CountAsync(countSpec);
            var ratings = await _unitOfWork.DoctorRatingRepository.GetAllAsync(specification);
            
            var data = _mapper.Map<IEnumerable<DoctorRatingDTO>>(ratings);
            
            return new PaginatedResult<DoctorRatingDTO>(pageIndex, pageSize, totalItems, data);
        }

        public async Task<DoctorRatingDTO> GetDoctorRatingByDoctorAndOwnerAsync(Guid doctorId, Guid petOwnerId)
        {
            _logger.LogInformation("Getting doctor rating for doctor ID: {DoctorId} and pet owner ID: {PetOwnerId}", 
                doctorId, petOwnerId);
            
            var specification = new DoctorRatingByDoctorAndOwnerSpecification1(doctorId, petOwnerId);
            var rating = await _unitOfWork.DoctorRatingRepository.GetAsync(specification);
            
            if (rating == null)
            {
                _logger.LogWarning("Doctor rating for doctor {DoctorId} and pet owner {PetOwnerId} not found", 
                    doctorId, petOwnerId);
                return null;
            }
            
            return _mapper.Map<DoctorRatingDTO>(rating);
        }

        public async Task<DoctorRatingDTO> CreateDoctorRatingAsync(DoctorRatingCreateDTO createDto)
        {
            _logger.LogInformation("Creating doctor rating for doctor ID: {DoctorId} and pet owner ID: {PetOwnerId}", 
                createDto.DoctorId, createDto.PetOwnerId);
            
            // Check if doctor exists
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(createDto.DoctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found", createDto.DoctorId);
                throw new KeyNotFoundException($"Doctor with ID {createDto.DoctorId} not found");
            }
            
            // Check if pet owner exists
            var petOwner = await _unitOfWork.PetOwnerRepository.GetByIdAsync(createDto.PetOwnerId);
            if (petOwner == null)
            {
                _logger.LogWarning("Pet owner with ID {PetOwnerId} not found", createDto.PetOwnerId);
                throw new KeyNotFoundException($"Pet owner with ID {createDto.PetOwnerId} not found");
            }
            
            // Check if rating already exists
            var existingRating = await GetDoctorRatingByDoctorAndOwnerAsync(createDto.DoctorId, createDto.PetOwnerId);
            if (existingRating != null)
            {
                _logger.LogWarning("Doctor rating already exists for doctor {DoctorId} and pet owner {PetOwnerId}", 
                    createDto.DoctorId, createDto.PetOwnerId);
                throw new InvalidOperationException("You have already rated this doctor");
            }
            
            var rating = new Core.Domain.Entities.HealthcareEntities.DoctorRating
            {
                DoctorId = createDto.DoctorId,
                PetOwnerId = createDto.PetOwnerId,
                Rating = createDto.Rating,
                Comment = createDto.Comment,
                CreatedDate = DateTime.UtcNow
            };
            
            await _unitOfWork.DoctorRatingRepository.AddAsync(rating);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetDoctorRatingByIdAsync(rating.Id);
        }

        public async Task<DoctorRatingDTO> UpdateDoctorRatingAsync(Guid id, DoctorRatingUpdateDTO updateDto)
        {
            _logger.LogInformation("Updating doctor rating with ID: {Id}", id);
            
            var rating = await _unitOfWork.DoctorRatingRepository.GetByIdAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Doctor rating with ID {Id} not found", id);
                throw new KeyNotFoundException($"Doctor rating with ID {id} not found");
            }
            
            rating.Rating = updateDto.Rating;
            rating.Comment = updateDto.Comment;
            rating.UpdatedDate = DateTime.UtcNow;
            
            _unitOfWork.DoctorRatingRepository.Update(rating);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetDoctorRatingByIdAsync(id);
        }

        public async Task DeleteDoctorRatingAsync(Guid id)
        {
            _logger.LogInformation("Deleting doctor rating with ID: {Id}", id);
            
            var rating = await _unitOfWork.DoctorRatingRepository.GetByIdAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Doctor rating with ID {Id} not found", id);
                throw new KeyNotFoundException($"Doctor rating with ID {id} not found");
            }
            
            _unitOfWork.DoctorRatingRepository.Delete(rating);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<double> GetAverageDoctorRatingAsync(Guid doctorId)
        {
            _logger.LogInformation("Getting average rating for doctor ID: {DoctorId}", doctorId);
            
            var specification = new DoctorRatingByDoctorIdSpecification1(doctorId);
            var ratings = await _unitOfWork.DoctorRatingRepository.GetAllAsync(specification);
            
            if (!ratings.Any())
            {
                return 0;
            }
            
            return ratings.Average(r => r.Rating);
        }

        public async Task<IEnumerable<DoctorWithRatingDTO>> GetTopRatedDoctorsAsync(int count = 10)
        {
            var doctors = await _unitOfWork.DoctorRepository.GetAllAsync();
            var result = new List<DoctorWithRatingDTO>();
            
            foreach (var doctor in doctors)
            {
                var avgRating = await GetAverageDoctorRatingAsync(doctor.Id);
                if (avgRating > 0) // Only include doctors with ratings
                {
                    result.Add(new DoctorWithRatingDTO
                    {
                        Id = doctor.Id,
                        FullName = doctor.FullName,
                        Specialization = doctor.Specialization,
                        AverageRating = avgRating,
                        ClinicId = doctor.ClinicId,
                        ClinicName = doctor.Clinic?.Name
                    });
                }
            }
            
            return result.OrderByDescending(d => d.AverageRating).Take(count);
        }

        #endregion

        #region Clinic Ratings

        public async Task<ClinicRatingDTO> GetClinicRatingByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting clinic rating with ID: {Id}", id);
            
            var specification = new BaseSpecification<Core.Domain.Entities.HealthcareEntities.ClinicRating>(r => r.Id == id);
            specification.AddInclude(r => r.Clinic);
            specification.AddInclude(r => r.PetOwner);
            
            var rating = await _unitOfWork.ClinicRatingRepository.GetAsync(specification);
            
            if (rating == null)
            {
                _logger.LogWarning("Clinic rating with ID {Id} not found", id);
                return null;
            }
            
            return _mapper.Map<ClinicRatingDTO>(rating);
        }

        public async Task<IEnumerable<ClinicRatingDTO>> GetClinicRatingsByClinicIdAsync(Guid clinicId)
        {
            _logger.LogInformation("Getting clinic ratings for clinic ID: {ClinicId}", clinicId);
            
            var specification = new ClinicRatingByClinicIdSpecification1(clinicId);
            var ratings = await _unitOfWork.ClinicRatingRepository.GetAllAsync(specification);
            
            return _mapper.Map<IEnumerable<ClinicRatingDTO>>(ratings);
        }

        public async Task<PaginatedResult<ClinicRatingDTO>> GetPagedClinicRatingsAsync(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Getting paged clinic ratings. Page: {PageIndex}, Size: {PageSize}", pageIndex, pageSize);
            
            var specification = new ClinicRatingPaginatedSpecification1(pageIndex, pageSize);
            var countSpec = new BaseSpecification<Core.Domain.Entities.HealthcareEntities.ClinicRating>();
            
            var totalItems = await _unitOfWork.ClinicRatingRepository.CountAsync(countSpec);
            var ratings = await _unitOfWork.ClinicRatingRepository.GetAllAsync(specification);
            
            var data = _mapper.Map<IEnumerable<ClinicRatingDTO>>(ratings);
            
            return new PaginatedResult<ClinicRatingDTO>(pageIndex, pageSize, totalItems, data);
        }

        public async Task<ClinicRatingDTO> GetClinicRatingByClinicAndOwnerAsync(Guid clinicId, Guid petOwnerId)
        {
            _logger.LogInformation("Getting clinic rating for clinic ID: {ClinicId} and pet owner ID: {PetOwnerId}", 
                clinicId, petOwnerId);
            
            var specification = new ClinicRatingByClinicAndOwnerSpecification1(clinicId, petOwnerId);
            var rating = await _unitOfWork.ClinicRatingRepository.GetAsync(specification);
            
            if (rating == null)
            {
                _logger.LogWarning("Clinic rating for clinic {ClinicId} and pet owner {PetOwnerId} not found", 
                    clinicId, petOwnerId);
                return null;
            }
            
            return _mapper.Map<ClinicRatingDTO>(rating);
        }

        public async Task<ClinicRatingDTO> CreateClinicRatingAsync(ClinicRatingCreateDTO createDto)
        {
            _logger.LogInformation("Creating clinic rating for clinic ID: {ClinicId} and pet owner ID: {PetOwnerId}", 
                createDto.ClinicId, createDto.PetOwnerId);
            
            // Check if clinic exists
            var clinic = await _unitOfWork.ClinicRepository.GetByIdAsync(createDto.ClinicId);
            if (clinic == null)
            {
                _logger.LogWarning("Clinic with ID {ClinicId} not found", createDto.ClinicId);
                throw new KeyNotFoundException($"Clinic with ID {createDto.ClinicId} not found");
            }
            
            // Check if pet owner exists
            var petOwner = await _unitOfWork.PetOwnerRepository.GetByIdAsync(createDto.PetOwnerId);
            if (petOwner == null)
            {
                _logger.LogWarning("Pet owner with ID {PetOwnerId} not found", createDto.PetOwnerId);
                throw new KeyNotFoundException($"Pet owner with ID {createDto.PetOwnerId} not found");
            }
            
            // Check if rating already exists
            var existingRating = await GetClinicRatingByClinicAndOwnerAsync(createDto.ClinicId, createDto.PetOwnerId);
            if (existingRating != null)
            {
                _logger.LogWarning("Clinic rating already exists for clinic {ClinicId} and pet owner {PetOwnerId}", 
                    createDto.ClinicId, createDto.PetOwnerId);
                throw new InvalidOperationException("You have already rated this clinic");
            }
            
            var rating = new Core.Domain.Entities.HealthcareEntities.ClinicRating
            {
                ClinicId = createDto.ClinicId,
                PetOwnerId = createDto.PetOwnerId,
                Rating = createDto.Rating,
                Comment = createDto.Comment,
                CreatedDate = DateTime.UtcNow
            };
            
            await _unitOfWork.ClinicRatingRepository.AddAsync(rating);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetClinicRatingByIdAsync(rating.Id);
        }

        public async Task<ClinicRatingDTO> UpdateClinicRatingAsync(Guid id, ClinicRatingUpdateDTO updateDto)
        {
            _logger.LogInformation("Updating clinic rating with ID: {Id}", id);
            
            var rating = await _unitOfWork.ClinicRatingRepository.GetByIdAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Clinic rating with ID {Id} not found", id);
                throw new KeyNotFoundException($"Clinic rating with ID {id} not found");
            }
            
            rating.Rating = updateDto.Rating;
            rating.Comment = updateDto.Comment;
            rating.UpdatedDate = DateTime.UtcNow;
            
            _unitOfWork.ClinicRatingRepository.Update(rating);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetClinicRatingByIdAsync(id);
        }

        public async Task DeleteClinicRatingAsync(Guid id)
        {
            _logger.LogInformation("Deleting clinic rating with ID: {Id}", id);
            
            var rating = await _unitOfWork.ClinicRatingRepository.GetByIdAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Clinic rating with ID {Id} not found", id);
                throw new KeyNotFoundException($"Clinic rating with ID {id} not found");
            }
            
            _unitOfWork.ClinicRatingRepository.Delete(rating);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<double> GetAverageClinicRatingAsync(Guid clinicId)
        {
            _logger.LogInformation("Getting average rating for clinic ID: {ClinicId}", clinicId);
            
            var specification = new ClinicRatingByClinicIdSpecification1(clinicId);
            var ratings = await _unitOfWork.ClinicRatingRepository.GetAllAsync(specification);
            
            if (!ratings.Any())
            {
                return 0;
            }
            
            return ratings.Average(r => r.Rating);
        }

        public async Task<IEnumerable<ClinicWithRatingDTO>> GetTopRatedClinicsAsync(int count = 10)
        {
            var clinics = await _unitOfWork.ClinicRepository.GetAllAsync();
            var result = new List<ClinicWithRatingDTO>();
            
            foreach (var clinic in clinics)
            {
                var avgRating = await GetAverageClinicRatingAsync(clinic.Id);
                if (avgRating > 0) // Only include clinics with ratings
                {
                    result.Add(new ClinicWithRatingDTO
                    {
                        Id = clinic.Id,
                        Name = clinic.Name,
                        Address = clinic.Address,
                        AverageRating = avgRating
                    });
                }
            }
            
            return result.OrderByDescending(c => c.AverageRating).Take(count);
        }

        #endregion

        #region Rating Trends

        public async Task<IEnumerable<RatingTrendDTO>> GetDoctorRatingTrendsAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
                throw new NotFoundException($"Doctor with ID {doctorId} not found");
        
            var spec = new DoctorRatingByDoctorIdSpecification1(doctorId);
            var ratings = await _unitOfWork.DoctorRatingRepository.GetAllWithSpecAsync(spec);
        
            var filteredRatings = ratings.Where(r => r.CreatedDate >= startDate && r.CreatedDate <= endDate)
                                        .OrderBy(r => r.CreatedDate);
        
            var result = new List<RatingTrendDTO>();
            var monthGroups = filteredRatings.GroupBy(r => new { r.CreatedDate.Year, r.CreatedDate.Month });
        
            foreach (var group in monthGroups)
            {
                var periodDate = new DateTime(group.Key.Year, group.Key.Month, 1);
                var averageRating = group.Average(r => r.Rating);
            
                result.Add(new RatingTrendDTO
                {
                    Period = periodDate,
                    AverageRating = averageRating,
                    Count = group.Count()
                });
            }
        
            return result;
        }

        public async Task<IEnumerable<RatingTrendDTO>> GetClinicRatingTrendsAsync(Guid clinicId, DateTime startDate, DateTime endDate)
        {
            var clinic = await _unitOfWork.ClinicRepository.GetByIdAsync(clinicId);
            if (clinic == null)
                throw new NotFoundException($"Clinic with ID {clinicId} not found");
        
            var spec = new ClinicRatingByClinicIdSpecification1(clinicId);
            var ratings = await _unitOfWork.ClinicRatingRepository.GetAllWithSpecAsync(spec);
        
            var filteredRatings = ratings.Where(r => r.CreatedDate >= startDate && r.CreatedDate <= endDate)
                                        .OrderBy(r => r.CreatedDate);
        
            var result = new List<RatingTrendDTO>();
            var monthGroups = filteredRatings.GroupBy(r => new { r.CreatedDate.Year, r.CreatedDate.Month });
        
            foreach (var group in monthGroups)
            {
                var periodDate = new DateTime(group.Key.Year, group.Key.Month, 1);
                var averageRating = group.Average(r => r.Rating);
            
                result.Add(new RatingTrendDTO
                {
                    Period = periodDate,
                    AverageRating = averageRating,
                    Count = group.Count()
                });
            }
        
            return result;
        }

        #endregion

        #region Additional Methods

        public async Task<PagedResultDTO<DoctorRatingDTO>> GetDoctorRatingsAsync(Guid doctorId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting doctor ratings. Doctor: {DoctorId}, Page: {PageIndex}, Size: {PageSize}", 
                doctorId, pageIndex, pageSize);
            
            var specification = new DoctorRatingByDoctorIdSpecification1(doctorId);
            specification.ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            var ratings = await _unitOfWork.DoctorRatingRepository.GetAllAsync(specification);
            
            var countSpec = new DoctorRatingByDoctorIdSpecification1(doctorId);
            var totalItems = await _unitOfWork.DoctorRatingRepository.CountAsync(countSpec);
            
            var items = _mapper.Map<IEnumerable<DoctorRatingDTO>>(ratings);
            
            return new PagedResultDTO<DoctorRatingDTO>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalItems
            };
        }

        public async Task<DoctorRatingSummaryDTO> GetDoctorRatingSummaryAsync(Guid doctorId)
        {
            _logger.LogInformation("Getting doctor rating summary for doctor {DoctorId}", doctorId);
            
            var specification = new DoctorRatingByDoctorIdSpecification1(doctorId);
            var ratings = await _unitOfWork.DoctorRatingRepository.GetAllAsync(specification);
            
            if (!ratings.Any())
            {
                return new DoctorRatingSummaryDTO
                {
                    DoctorId = doctorId,
                    AverageRating = 0,
                    TotalRatings = 0,
                    RatingDistribution = new Dictionary<int, int>
                    {
                        {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}
                    }
                };
            }
            
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(doctorId);
            
            var distribution = new Dictionary<int, int>
            {
                {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}
            };
            
            foreach (var rating in ratings)
            {
                if (rating.Rating >= 1 && rating.Rating <= 5)
                {
                    distribution[rating.Rating]++;
                }
            }
            
            return new DoctorRatingSummaryDTO
            {
                DoctorId = doctorId,
                DoctorName = doctor?.FullName,
                AverageRating = ratings.Average(r => r.Rating),
                TotalRatings = ratings.Count(),
                RatingDistribution = distribution
            };
        }

        public async Task<DoctorRatingDTO> GetDoctorRatingByIdAsync(Guid doctorId, Guid ratingId)
        {
            _logger.LogInformation("Getting doctor rating with ID {RatingId} for doctor {DoctorId}", ratingId, doctorId);
            
            var specification = new DoctorRatingByIdSpecification(ratingId);
            var rating = await _unitOfWork.DoctorRatingRepository.GetAsync(specification);
            
            if (rating == null || rating.DoctorId != doctorId)
            {
                return null;
            }
            
            return _mapper.Map<DoctorRatingDTO>(rating);
        }

        public async Task FlagInappropriateDoctorRatingAsync(Guid doctorId, Guid ratingId, string reason)
        {
            _logger.LogInformation("Flagging doctor rating {RatingId} as inappropriate for doctor {DoctorId}", ratingId, doctorId);
            
            var rating = await GetDoctorRatingByIdAsync(doctorId, ratingId);
            if (rating == null)
            {
                throw new NotFoundException($"Rating with ID {ratingId} not found for doctor {doctorId}");
            }
            
            // In a real implementation, this would create a flag record in the database and notify administrators
            
            _logger.LogInformation("Rating {RatingId} flagged as inappropriate. Reason: {Reason}", ratingId, reason);
        }

        public async Task<PagedResultDTO<ClinicRatingDTO>> GetClinicRatingsAsync(Guid clinicId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting clinic ratings. Clinic: {ClinicId}, Page: {PageIndex}, Size: {PageSize}", 
                clinicId, pageIndex, pageSize);
            
            var specification = new ClinicRatingByClinicIdSpecification1(clinicId);
            specification.ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            var ratings = await _unitOfWork.ClinicRatingRepository.GetAllAsync(specification);
            
            var countSpec = new ClinicRatingByClinicIdSpecification1(clinicId);
            var totalItems = await _unitOfWork.ClinicRatingRepository.CountAsync(countSpec);
            
            var items = _mapper.Map<IEnumerable<ClinicRatingDTO>>(ratings);
            
            return new PagedResultDTO<ClinicRatingDTO>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalItems
            };
        }

        public async Task<ClinicRatingSummaryDTO> GetClinicRatingSummaryAsync(Guid clinicId)
        {
            _logger.LogInformation("Getting clinic rating summary for clinic {ClinicId}", clinicId);
            
            var specification = new ClinicRatingByClinicIdSpecification1(clinicId);
            var ratings = await _unitOfWork.ClinicRatingRepository.GetAllAsync(specification);
            
            if (!ratings.Any())
            {
                return new ClinicRatingSummaryDTO
                {
                    ClinicId = clinicId,
                    AverageRating = 0,
                    TotalRatings = 0,
                    RatingDistribution = new Dictionary<int, int>
                    {
                        {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}
                    }
                };
            }
            
            var clinic = await _unitOfWork.ClinicRepository.GetByIdAsync(clinicId);
            
            var distribution = new Dictionary<int, int>
            {
                {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}
            };
            
            foreach (var rating in ratings)
            {
                if (rating.Rating >= 1 && rating.Rating <= 5)
                {
                    distribution[rating.Rating]++;
                }
            }
            
            return new ClinicRatingSummaryDTO
            {
                ClinicId = clinicId,
                ClinicName = clinic?.Name,
                AverageRating = ratings.Average(r => r.Rating),
                TotalRatings = ratings.Count(),
                RatingDistribution = distribution
            };
        }

        public async Task<ClinicRatingDTO> GetClinicRatingByIdAsync(Guid clinicId, Guid ratingId)
        {
            _logger.LogInformation("Getting clinic rating with ID {RatingId} for clinic {ClinicId}", ratingId, clinicId);
            
            var specification = new ClinicRatingByIdSpecification(ratingId);
            var rating = await _unitOfWork.ClinicRatingRepository.GetAsync(specification);
            
            if (rating == null || rating.ClinicId != clinicId)
            {
                return null;
            }
            
            return _mapper.Map<ClinicRatingDTO>(rating);
        }

        public async Task FlagInappropriateClinicRatingAsync(Guid clinicId, Guid ratingId, string reason)
        {
            _logger.LogInformation("Flagging clinic rating {RatingId} as inappropriate for clinic {ClinicId}", ratingId, clinicId);
            
            var rating = await GetClinicRatingByIdAsync(clinicId, ratingId);
            if (rating == null)
            {
                throw new NotFoundException($"Rating with ID {ratingId} not found for clinic {clinicId}");
            }
            
            // In a real implementation, this would create a flag record in the database and notify administrators
            
            _logger.LogInformation("Rating {RatingId} flagged as inappropriate. Reason: {Reason}", ratingId, reason);
        }

        public async Task<IEnumerable<DoctorWithRatingDTO>> GetTopRatedDoctorsAsync(int count, string specialty)
        {
            _logger.LogInformation("Getting top-rated doctors. Count: {Count}, Specialty: {Specialty}", count, specialty);
            
            var doctors = await _unitOfWork.DoctorRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(specialty))
            {
                doctors = doctors.Where(d => d.Specialization.Equals(specialty, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            var result = new List<DoctorWithRatingDTO>();
            
            foreach (var doctor in doctors)
            {
                var avgRating = await GetAverageDoctorRatingAsync(doctor.Id);
                if (avgRating > 0) // Only include doctors with ratings
                {
                    result.Add(new DoctorWithRatingDTO
                    {
                        Id = doctor.Id,
                        FullName = doctor.FullName,
                        Specialization = doctor.Specialization,
                        AverageRating = avgRating,
                        ClinicId = doctor.ClinicId,
                        ClinicName = doctor.Clinic?.Name
                    });
                }
            }
            
            return result.OrderByDescending(d => d.AverageRating).Take(count);
        }

        public async Task<IEnumerable<ClinicWithRatingDTO>> GetTopRatedClinicsAsync(int count, string city)
        {
            _logger.LogInformation("Getting top-rated clinics. Count: {Count}, City: {City}", count, city);
            
            var clinics = await _unitOfWork.ClinicRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(city))
            {
                clinics = clinics.Where(c => c.Address.Contains(city, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            var result = new List<ClinicWithRatingDTO>();
            
            foreach (var clinic in clinics)
            {
                var avgRating = await GetAverageClinicRatingAsync(clinic.Id);
                if (avgRating > 0) // Only include clinics with ratings
                {
                    result.Add(new ClinicWithRatingDTO
                    {
                        Id = clinic.Id,
                        Name = clinic.Name,
                        Address = clinic.Address,
                        AverageRating = avgRating
                    });
                }
            }
            
            return result.OrderByDescending(c => c.AverageRating).Take(count);
        }

        #endregion
    }

    #region Custom exceptions

    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base(string.Join(", ", errors))
        {
            Errors = errors;
        }

        public ValidationException(string error)
            : this(new[] { error })
        {
        }
    }
    #endregion
} 
