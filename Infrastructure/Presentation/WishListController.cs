using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.WishListModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishListController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public WishListController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/WishList
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<WishListDTO>> GetUserWishList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishList = await _serviceManager.WishListService.GetUserWishListAsync(userId);
            return Ok(wishList);
        }

        // GET: api/WishList/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<WishListDTO>> GetWishListById(int id)
        {
            try
            {
                var wishList = await _serviceManager.WishListService.GetWishListByIdAsync(id);
                
                // Verify ownership or admin access
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (wishList.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                return Ok(wishList);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/WishList/items
        [HttpPost("items")]
        [Authorize]
        public async Task<ActionResult<WishListItemDTO>> AddItemToWishList([FromBody] AddToWishListDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var item = await _serviceManager.WishListService.AddItemToWishListAsync(userId, model.ProductId);
                return CreatedAtAction(nameof(GetUserWishList), item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/WishList/items/{id}
        [HttpDelete("items/{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveItemFromWishList(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _serviceManager.WishListService.RemoveItemFromWishListAsync(userId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);
                
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/WishList
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> ClearWishList()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _serviceManager.WishListService.ClearWishListAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/WishList/check/{productId}
        [HttpGet("check/{productId}")]
        [Authorize]
        public async Task<ActionResult<bool>> IsProductInWishList(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isInWishList = await _serviceManager.WishListService.IsProductInWishListAsync(userId, productId);
            return Ok(isInWishList);
        }
    }
} 