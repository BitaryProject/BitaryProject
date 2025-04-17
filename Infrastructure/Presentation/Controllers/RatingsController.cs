/*
using Infrastructure.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IDoctorService _doctorService;
        private readonly IClinicService _clinicService;
        private readonly IPetOwnerService _petOwnerService;
        private readonly ILogger<RatingsController> _logger;

        public RatingsController(
            IRatingService ratingService,
            IDoctorService doctorService,
            IClinicService clinicService,
            IPetOwnerService petOwnerService,
            ILogger<RatingsController> logger)
        {
            _ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
            _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
            _clinicService = clinicService ?? throw new ArgumentNullException(nameof(clinicService));
            _petOwnerService = petOwnerService ?? throw new ArgumentNullException(nameof(petOwnerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Doctor Ratings

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<PagedResultDTO<DoctorRatingDTO>>> GetDoctorRatings(
            Guid doctorId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");

                var ratings = await _ratingService.GetDoctorRatingsAsync(doctorId, pageIndex, pageSize);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/{doctorId}/summary")]
        public async Task<ActionResult<DoctorRatingSummaryDTO>> GetDoctorRatingSummary(Guid doctorId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");

                var summary = await _ratingService.GetDoctorRatingSummaryAsync(doctorId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating summary for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("doctor/{doctorId}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<DoctorRatingDTO>> RateDoctor(Guid doctorId, [FromBody] DoctorRatingCreateDTO ratingDto)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");

                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");

                // Verify the pet owner has had an appointment with this doctor
                var hasAppointment = await _doctorService.HasPetOwnerHadAppointmentWithDoctorAsync(petOwner.Id, doctorId);
                if (!hasAppointment)
                    return BadRequest("You can only rate doctors you've had appointments with");

                ratingDto.PetOwnerId = petOwner.Id;
                var rating = await _ratingService.CreateDoctorRatingAsync(ratingDto);
                
                return CreatedAtAction(
                    nameof(GetDoctorRatingById), 
                    new { doctorId, id = rating.Id }, 
                    rating);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rating doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/{doctorId}/rating/{id}")]
        public async Task<ActionResult<DoctorRatingDTO>> GetDoctorRatingById(Guid doctorId, Guid id)
        {
            try
            {
                var rating = await _ratingService.GetDoctorRatingByIdAsync(id);
                
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found for doctor {doctorId}");
                    
                if (rating.DoctorId != doctorId)
                    return NotFound($"Rating with ID {id} is not associated with doctor {doctorId}");
                
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating {RatingId} for doctor {DoctorId}", id, doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("doctor/{doctorId}/rating/{id}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult> UpdateDoctorRating(Guid doctorId, Guid id, [FromBody] DoctorRatingUpdateDTO ratingDto)
        {
            try
            {
                var rating = await _ratingService.GetDoctorRatingByIdAsync(id);
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found");
                    
                if (rating.DoctorId != doctorId)
                    return NotFound($"Rating with ID {id} is not associated with doctor {doctorId}");

                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");

                // Verify the pet owner is updating their own rating
                if (rating.PetOwnerId != petOwner.Id)
                    return Forbid("You can only update your own ratings");

                await _ratingService.UpdateDoctorRatingAsync(id, ratingDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rating {RatingId} for doctor {DoctorId}", id, doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("doctor/{doctorId}/rating/{id}")]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult> DeleteDoctorRating(Guid doctorId, Guid id)
        {
            try
            {
                var rating = await _ratingService.GetDoctorRatingByIdAsync(id);
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found");
                    
                if (rating.DoctorId != doctorId)
                    return NotFound($"Rating with ID {id} is not associated with doctor {doctorId}");

                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");

                    // Verify the pet owner is deleting their own rating
                    if (rating.PetOwnerId != petOwner.Id)
                        return Forbid("You can only delete your own ratings");
                }

                await _ratingService.DeleteDoctorRatingAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rating {RatingId} for doctor {DoctorId}", id, doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        #endregion

        #region Clinic Ratings

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<PagedResultDTO<ClinicRatingDTO>>> GetClinicRatings(
            Guid clinicId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");

                var ratings = await _ratingService.GetClinicRatingsAsync(clinicId, pageIndex, pageSize);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinic/{clinicId}/summary")]
        public async Task<ActionResult<ClinicRatingSummaryDTO>> GetClinicRatingSummary(Guid clinicId)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");

                var summary = await _ratingService.GetClinicRatingSummaryAsync(clinicId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating summary for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("clinic/{clinicId}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<ClinicRatingDTO>> RateClinic(Guid clinicId, [FromBody] ClinicRatingCreateDTO ratingDto)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");

                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");

                // Verify the pet owner has visited this clinic
                bool hasVisitedClinic = await _clinicService.HasPetOwnerVisitedClinicAsync(petOwner.Id, clinicId);
                if (!hasVisitedClinic)
                    return BadRequest("You can only rate clinics you've visited");

                ratingDto.ClinicId = clinicId;
                ratingDto.PetOwnerId = petOwner.Id;
                
                var rating = await _ratingService.CreateClinicRatingAsync(ratingDto);
                
                return CreatedAtAction(
                    nameof(GetClinicRatingById), 
                    new { clinicId, id = rating.Id }, 
                    rating);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rating clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinic/{clinicId}/rating/{id}")]
        public async Task<ActionResult<ClinicRatingDTO>> GetClinicRatingById(Guid clinicId, Guid id)
        {
            try
            {
                var rating = await _ratingService.GetClinicRatingByIdAsync(id);
                
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found");
                    
                if (rating.ClinicId != clinicId)
                    return NotFound($"Rating with ID {id} is not associated with clinic {clinicId}");
                
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating {RatingId} for clinic {ClinicId}", id, clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("clinic/{clinicId}/rating/{id}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult> UpdateClinicRating(Guid clinicId, Guid id, [FromBody] ClinicRatingUpdateDTO ratingDto)
        {
            try
            {
                var rating = await _ratingService.GetClinicRatingByIdAsync(id);
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found");
                    
                if (rating.ClinicId != clinicId)
                    return NotFound($"Rating with ID {id} is not associated with clinic {clinicId}");

                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");

                // Verify the pet owner is updating their own rating
                if (rating.PetOwnerId != petOwner.Id)
                    return Forbid("You can only update your own ratings");

                await _ratingService.UpdateClinicRatingAsync(id, ratingDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rating {RatingId} for clinic {ClinicId}", id, clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("clinic/{clinicId}/rating/{id}")]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult> DeleteClinicRating(Guid clinicId, Guid id)
        {
            try
            {
                var rating = await _ratingService.GetClinicRatingByIdAsync(id);
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found");
                    
                if (rating.ClinicId != clinicId)
                    return NotFound($"Rating with ID {id} is not associated with clinic {clinicId}");

                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");

                    // Verify the pet owner is deleting their own rating
                    if (rating.PetOwnerId != petOwner.Id)
                        return Forbid("You can only delete your own ratings");
                }

                await _ratingService.DeleteClinicRatingAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rating {RatingId} for clinic {ClinicId}", id, clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        #endregion

        #region Reports and Analytics

        [HttpGet("top-doctors")]
        public async Task<ActionResult<IEnumerable<DoctorRatingDTO>>> GetTopRatedDoctors(
            [FromQuery] int limit = 10,
            [FromQuery] string? specialty = null)
        {
            try
            {
                var doctors = await _ratingService.GetTopRatedDoctorsAsync(limit, specialty);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top-rated doctors");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("top-clinics")]
        public async Task<ActionResult<IEnumerable<ClinicRatingDTO>>> GetTopRatedClinics(
            [FromQuery] int limit = 10,
            [FromQuery] string? city = null)
        {
            try
            {
                var clinics = await _ratingService.GetTopRatedClinicsAsync(limit, city);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top-rated clinics");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/{doctorId}/rating-trends")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<RatingTrendDTO>>> GetDoctorRatingTrends(
            Guid doctorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");

                // If doctor is requesting their own trends
                if (User.IsInRole("Doctor"))
                {
                    var userId = User.GetUserId();
                    var requestingDoctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                    
                    if (requestingDoctor == null || requestingDoctor.Id != doctorId)
                        return Forbid("You can only view your own rating trends");
                }

                var trends = await _ratingService.GetDoctorRatingTrendsAsync(
                    doctorId, 
                    startDate, 
                    endDate ?? DateTime.Now);
                    
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating trends for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinic/{clinicId}/rating-trends")]
        [Authorize(Roles = "Admin,ClinicAdmin")]
        public async Task<ActionResult<IEnumerable<RatingTrendDTO>>> GetClinicRatingTrends(
            Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");

                // If clinic admin is requesting their own clinic's trends
                if (User.IsInRole("ClinicAdmin"))
                {
                    var userId = User.GetUserId();
                    var isAdmin = await _clinicService.IsUserClinicAdminAsync(userId, clinicId);
                    
                    if (!isAdmin)
                        return Forbid("You can only view rating trends for clinics you administer");
                }

                var trends = await _ratingService.GetClinicRatingTrendsAsync(
                    clinicId, 
                    startDate, 
                    endDate ?? DateTime.Now);
                    
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating trends for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        #endregion

        #region Flag Inappropriate Reviews

        [HttpPost("doctor/{doctorId}/rating/{id}/flag")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult> FlagInappropriateDoctorRating(
            Guid doctorId, 
            Guid id, 
            [FromBody] string reason)
        {
            try
            {
                var rating = await _ratingService.GetDoctorRatingByIdAsync(doctorId, id);
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found for doctor {doctorId}");

                await _ratingService.FlagInappropriateDoctorRatingAsync(doctorId, id, reason);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flagging doctor rating {RatingId} as inappropriate", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("clinic/{clinicId}/rating/{id}/flag")]
        [Authorize(Roles = "ClinicAdmin,Admin")]
        public async Task<ActionResult> FlagInappropriateClinicRating(
            Guid clinicId, 
            Guid id, 
            [FromBody] string reason)
        {
            try
            {
                var rating = await _ratingService.GetClinicRatingByIdAsync(clinicId, id);
                if (rating == null)
                    return NotFound($"Rating with ID {id} not found for clinic {clinicId}");

                await _ratingService.FlagInappropriateClinicRatingAsync(clinicId, id, reason);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flagging clinic rating {RatingId} as inappropriate", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        #endregion
    }
} 


*/
