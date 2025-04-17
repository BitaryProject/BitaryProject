using Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IPetOwnerService _petOwnerService;
        private readonly ILogger<RatingController> _logger;

        public RatingController(
            IRatingService ratingService,
            IPetOwnerService petOwnerService,
            ILogger<RatingController> logger)
        {
            _ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
            _petOwnerService = petOwnerService ?? throw new ArgumentNullException(nameof(petOwnerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Doctor Ratings

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetDoctorRatings(Guid doctorId)
        {
            try
            {
                var ratings = await _ratingService.GetDoctorRatingsByDoctorIdAsync(doctorId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor ratings for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving doctor ratings");
            }
        }

        [HttpGet("doctor/{doctorId}/average")]
        public async Task<IActionResult> GetAverageDoctorRating(Guid doctorId)
        {
            try
            {
                var averageRating = await _ratingService.GetAverageDoctorRatingAsync(doctorId);
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving average rating for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving average doctor rating");
            }
        }

        [HttpGet("doctor/paged")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedDoctorRatings([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedRatings = await _ratingService.GetPagedDoctorRatingsAsync(pageIndex, pageSize);
                return Ok(pagedRatings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged doctor ratings");
                return StatusCode(500, "An error occurred while retrieving paged doctor ratings");
            }
        }

        [HttpGet("doctor/detail/{id}")]
        public async Task<IActionResult> GetDoctorRatingById(Guid id)
        {
            try
            {
                var rating = await _ratingService.GetDoctorRatingByIdAsync(id);
                if (rating == null)
                {
                    return NotFound();
                }
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor rating with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving doctor rating");
            }
        }

        [HttpGet("doctor/{doctorId}/owner/{petOwnerId}")]
        public async Task<IActionResult> GetDoctorRatingByDoctorAndOwner(Guid doctorId, Guid petOwnerId)
        {
            try
            {
                var rating = await _ratingService.GetDoctorRatingByDoctorAndOwnerAsync(doctorId, petOwnerId);
                if (rating == null)
                {
                    return NotFound();
                }
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor rating for doctor {DoctorId} and owner {PetOwnerId}", doctorId, petOwnerId);
                return StatusCode(500, "An error occurred while retrieving doctor rating");
            }
        }

        [HttpPost("doctor")]
        public async Task<IActionResult> CreateDoctorRating([FromBody] DoctorRatingCreateDTO createDto)
        {
            try
            {
                var createdRating = await _ratingService.CreateDoctorRatingAsync(createDto);
                return CreatedAtAction(nameof(GetDoctorRatingById), new { id = createdRating.Id }, createdRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor rating");
                return StatusCode(500, "An error occurred while creating doctor rating");
            }
        }

        [HttpPut("doctor/{id}")]
        public async Task<IActionResult> UpdateDoctorRating(Guid id, [FromBody] DoctorRatingUpdateDTO updateDto)
        {
            try
            {
                var updatedRating = await _ratingService.UpdateDoctorRatingAsync(id, updateDto);
                return Ok(updatedRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor rating with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating doctor rating");
            }
        }

        [HttpDelete("doctor/{id}")]
        public async Task<IActionResult> DeleteDoctorRating(Guid id)
        {
            try
            {
                await _ratingService.DeleteDoctorRatingAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor rating with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting doctor rating");
            }
        }

        #endregion

        #region Clinic Ratings

        [HttpGet("clinic/{clinicId}")]
        public async Task<IActionResult> GetClinicRatings(Guid clinicId)
        {
            try
            {
                var ratings = await _ratingService.GetClinicRatingsByClinicIdAsync(clinicId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic ratings for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while retrieving clinic ratings");
            }
        }

        [HttpGet("clinic/{clinicId}/average")]
        public async Task<IActionResult> GetAverageClinicRating(Guid clinicId)
        {
            try
            {
                var averageRating = await _ratingService.GetAverageClinicRatingAsync(clinicId);
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving average rating for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while retrieving average clinic rating");
            }
        }

        [HttpGet("clinic/paged")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedClinicRatings([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedRatings = await _ratingService.GetPagedClinicRatingsAsync(pageIndex, pageSize);
                return Ok(pagedRatings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged clinic ratings");
                return StatusCode(500, "An error occurred while retrieving paged clinic ratings");
            }
        }

        [HttpGet("clinic/detail/{id}")]
        public async Task<IActionResult> GetClinicRatingById(Guid id)
        {
            try
            {
                var rating = await _ratingService.GetClinicRatingByIdAsync(id);
                if (rating == null)
                {
                    return NotFound();
                }
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic rating with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving clinic rating");
            }
        }

        [HttpGet("clinic/{clinicId}/owner/{petOwnerId}")]
        public async Task<IActionResult> GetClinicRatingByClinicAndOwner(Guid clinicId, Guid petOwnerId)
        {
            try
            {
                var rating = await _ratingService.GetClinicRatingByClinicAndOwnerAsync(clinicId, petOwnerId);
                if (rating == null)
                {
                    return NotFound();
                }
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic rating for clinic {ClinicId} and owner {PetOwnerId}", clinicId, petOwnerId);
                return StatusCode(500, "An error occurred while retrieving clinic rating");
            }
        }

        [HttpPost("clinic")]
        public async Task<IActionResult> CreateClinicRating([FromBody] ClinicRatingCreateDTO createDto)
        {
            try
            {
                var createdRating = await _ratingService.CreateClinicRatingAsync(createDto);
                return CreatedAtAction(nameof(GetClinicRatingById), new { id = createdRating.Id }, createdRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating clinic rating");
                return StatusCode(500, "An error occurred while creating clinic rating");
            }
        }

        [HttpPut("clinic/{id}")]
        public async Task<IActionResult> UpdateClinicRating(Guid id, [FromBody] ClinicRatingUpdateDTO updateDto)
        {
            try
            {
                var updatedRating = await _ratingService.UpdateClinicRatingAsync(id, updateDto);
                return Ok(updatedRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic rating with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating clinic rating");
            }
        }

        [HttpDelete("clinic/{id}")]
        public async Task<IActionResult> DeleteClinicRating(Guid id)
        {
            try
            {
                await _ratingService.DeleteClinicRatingAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting clinic rating with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting clinic rating");
            }
        }

        #endregion

        #region Analytics

        [HttpGet("doctors/top")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<DoctorRatingDTO>>> GetTopRatedDoctors(
            [FromQuery] int limit = 5,
            [FromQuery] string? specialty = null)
        {
            try
            {
                var doctors = await _ratingService.GetTopRatedDoctorsAsync(limit, specialty);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top rated doctors");
                return StatusCode(500, "An error occurred while retrieving top rated doctors");
            }
        }

        [HttpGet("clinics/top")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ClinicRatingDTO>>> GetTopRatedClinics(
            [FromQuery] int limit = 5,
            [FromQuery] string? city = null)
        {
            try
            {
                var clinics = await _ratingService.GetTopRatedClinicsAsync(limit, city);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top rated clinics");
                return StatusCode(500, "An error occurred while retrieving top rated clinics");
            }
        }

        [HttpGet("doctor/{doctorId}/trends")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<RatingTrendDTO>>> GetDoctorRatingTrends(
            Guid doctorId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddMonths(-6);
                var end = endDate ?? DateTime.UtcNow;

                if (start > end)
                {
                    return BadRequest("Start date must be before end date");
                }

                var trends = await _ratingService.GetDoctorRatingTrendsAsync(doctorId, start, end);
                return Ok(trends);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor rating trends");
                return StatusCode(500, "An error occurred while retrieving doctor rating trends");
            }
        }

        [HttpGet("clinic/{clinicId}/trends")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RatingTrendDTO>>> GetClinicRatingTrends(
            Guid clinicId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddMonths(-6);
                var end = endDate ?? DateTime.UtcNow;

                if (start > end)
                {
                    return BadRequest("Start date must be before end date");
                }

                var trends = await _ratingService.GetClinicRatingTrendsAsync(clinicId, start, end);
                return Ok(trends);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic rating trends");
                return StatusCode(500, "An error occurred while retrieving clinic rating trends");
            }
        }

        #endregion
    }
} 

