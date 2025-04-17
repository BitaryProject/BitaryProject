using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // Doctor ratings
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetDoctorRatings(Guid doctorId)
        {
            try
            {
                var ratings = await _ratingService.GetDoctorRatingsAsync(doctorId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("doctor/{doctorId}/average")]
        public async Task<ActionResult<double>> GetDoctorAverageRating(Guid doctorId)
        {
            try
            {
                var averageRating = await _ratingService.GetDoctorAverageRatingAsync(doctorId);
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("doctor/{doctorId}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<RatingDTO>> AddDoctorRating(Guid doctorId, RatingCreateDTO ratingDto)
        {
            try
            {
                var userId = User.GetUserId();
                var rating = await _ratingService.AddDoctorRatingAsync(doctorId, userId, ratingDto);
                return CreatedAtAction(nameof(GetDoctorRatings), new { doctorId }, rating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("doctor/{ratingId}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<RatingDTO>> UpdateDoctorRating(Guid ratingId, RatingCreateDTO ratingDto)
        {
            try
            {
                var rating = await _ratingService.UpdateDoctorRatingAsync(ratingId, ratingDto);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("doctor/{ratingId}")]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult<bool>> DeleteDoctorRating(Guid ratingId)
        {
            try
            {
                var result = await _ratingService.DeleteDoctorRatingAsync(ratingId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Clinic ratings
        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetClinicRatings(Guid clinicId)
        {
            try
            {
                var ratings = await _ratingService.GetClinicRatingsAsync(clinicId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("clinic/{clinicId}/average")]
        public async Task<ActionResult<double>> GetClinicAverageRating(Guid clinicId)
        {
            try
            {
                var averageRating = await _ratingService.GetClinicAverageRatingAsync(clinicId);
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("clinic/{clinicId}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<RatingDTO>> AddClinicRating(Guid clinicId, RatingCreateDTO ratingDto)
        {
            try
            {
                var userId = User.GetUserId();
                var rating = await _ratingService.AddClinicRatingAsync(clinicId, userId, ratingDto);
                return CreatedAtAction(nameof(GetClinicRatings), new { clinicId }, rating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("clinic/{ratingId}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<RatingDTO>> UpdateClinicRating(Guid ratingId, RatingCreateDTO ratingDto)
        {
            try
            {
                var rating = await _ratingService.UpdateClinicRatingAsync(ratingId, ratingDto);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("clinic/{ratingId}")]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult<bool>> DeleteClinicRating(Guid ratingId)
        {
            try
            {
                var result = await _ratingService.DeleteClinicRatingAsync(ratingId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // User's ratings
        [HttpGet("my/doctor")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetMyDoctorRatings()
        {
            try
            {
                var userId = User.GetUserId();
                var ratings = await _ratingService.GetUserDoctorRatingsAsync(userId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my/clinic")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetMyClinicRatings()
        {
            try
            {
                var userId = User.GetUserId();
                var ratings = await _ratingService.GetUserClinicRatingsAsync(userId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 