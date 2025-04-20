using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.OrderEntities;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]

    public class OrdersController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost]
        [AllowAnonymous]  // Allow anonymous access for testing
        public async Task<ActionResult<OrderResult>> Create([FromBody] OrderRequest request)
        {
            Console.WriteLine("Request reached Create method");
            // If user is authenticated, use their email, otherwise use a default
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "guest@example.com";
            
            Console.WriteLine($"Creating order with email: {email}");
            
            try
            {
                var order = await serviceManager.OrderService.CreateOrUpdateOrderAsync(request, email);
                return Ok(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Let the global exception handler handle it
            }
        }

        [HttpGet]//a
        public async Task<ActionResult<IEnumerable<OrderResult>>> GetOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "guest@example.com";
            Console.WriteLine($"GetOrders: Searching for orders with email: {email}");
            
            try
            {
                var orders = await serviceManager.OrderService.GetOrderByEmailAsync(email);
                Console.WriteLine($"GetOrders: Found {orders.Count()} orders for {email}");
                
                if (!orders.Any())
                {
                    Console.WriteLine("GetOrders: No orders found for email: " + email);
                    
                    // Try to create a detailed diagnostic response
                    var response = new
                    {
                        message = $"No orders found for user {email}",
                        userAuthenticated = User.Identity?.IsAuthenticated ?? false,
                        userEmail = email,
                        suggestion = "If you're expecting orders, please check that you're logged in with the correct account"
                    };
                    
                    return Ok(response);
                }
                
                // If we have orders, return them
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetOrders error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResult>> GetOrder(Guid id)
        {
            var orders = await serviceManager.OrderService.GetOrderByIdAsync(id);
            return Ok(orders);
        }

        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<DeliveryMethodResult>> GetDeliveryMethods()
        {
            var methods = await serviceManager.OrderService.GetDeliveryMethodResult();
            return Ok(methods);
        }

        [HttpGet("by-email/{email}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<OrderResult>>> GetOrdersByEmail(string email)
        {
            Console.WriteLine($"Getting orders for specific email: {email}");
            
            try
            {
                var orders = await serviceManager.OrderService.GetOrderByEmailAsync(email);
                Console.WriteLine($"Found {orders.Count()} orders for email: {email}");
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting orders by email: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Retrieves orders for the current authenticated user
        /// </summary>
        /// <returns>A collection of the user's orders</returns>
        [HttpGet("my-orders")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderResult>>> GetOrdersByUserEmail()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("User email not found in claims");
                    return Unauthorized(new { error = "User not authenticated properly" });
                }
                
                Console.WriteLine($"Getting orders for authenticated user with email: {email}");
                
                var orders = await serviceManager.OrderService.GetOrderByEmailAsync(email);
                
                Console.WriteLine($"Found {orders.Count()} orders for user {email}");
                
                // Add detailed information about order status distribution
                var statusCounts = orders
                    .GroupBy(o => o.PaymentStatus)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToList();
                
                foreach (var statusGroup in statusCounts)
                {
                    Console.WriteLine($"Status: {statusGroup.Status}, Count: {statusGroup.Count}");
                }
                
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrdersByUserEmail: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message });
            }
        }
        
        /// <summary>
        /// Retrieves all orders with optional filtering and pagination
        /// </summary>
        /// <param name="status">Optional filter by payment status: "Pending", "PaymentReceived", or "PaymentFailed"</param>
        /// <param name="pageNumber">Page number, starting at 1 (default: 1)</param>
        /// <param name="pageSize">Number of orders per page (default: 10)</param>
        /// <returns>A collection of orders matching the specified criteria</returns>
        /// <remarks>
        /// Sample request:
        /// GET /api/Orders/all?status=PaymentReceived&amp;pageNumber=1&amp;pageSize=10
        /// 
        /// Pagination metadata is included in the X-Pagination response header.
        /// </remarks>
        [HttpGet("all")]
        [AllowAnonymous] // Changed from [Authorize(Roles = "Admin")] for debugging purposes
        public async Task<ActionResult<IEnumerable<OrderResult>>> GetAllOrders(
            [FromQuery] string status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate input parameters
            if (pageNumber < 1)
            {
                return BadRequest(new { error = "Page number must be greater than or equal to 1" });
            }
            
            if (pageSize < 1 || pageSize > 50)
            {
                return BadRequest(new { error = "Page size must be between 1 and 50" });
            }
            
            // Validate status parameter if provided
            if (!string.IsNullOrEmpty(status))
            {
                // Check if status is a valid OrderPaymentStatus enum value
                if (!Enum.TryParse<Domain.Entities.OrderEntities.OrderPaymentStatus>(status, true, out _))
                {
                    return BadRequest(new { 
                        error = "Invalid status value. Valid values are: Pending, PaymentReceived, PaymentFailed" 
                    });
                }
            }
            
            Console.WriteLine($"Getting all orders with status filter: {status ?? "None"}, page: {pageNumber}, size: {pageSize}");
            
            try
            {
                // First, try to get orders for a specific email (for testing/debugging)
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(email))
                {
                    Console.WriteLine($"Authenticated user with email: {email}");
                    
                    // Get the user's orders first to check if we can retrieve any orders at all
                    var userOrders = await serviceManager.OrderService.GetOrderByEmailAsync(email);
                    Console.WriteLine($"User {email} has {userOrders.Count()} orders");
                }
                
                // Fall back to getting all orders regardless of email
                var orders = await serviceManager.OrderService.GetAllOrdersAsync(status, pageNumber, pageSize);
                
                // Check if orders are empty and log more details
                if (orders == null || !orders.Any())
                {
                    Console.WriteLine("WARNING: GetAllOrdersAsync returned null or empty result");
                    
                    // As a fallback, try to get any orders in the system
                    var guestOrders = await serviceManager.OrderService.GetOrderByEmailAsync("guest@example.com");
                    Console.WriteLine($"Fallback: Found {guestOrders.Count()} orders for guest@example.com");
                    
                    if (guestOrders.Any())
                    {
                        Console.WriteLine("Returning guest orders as fallback");
                        
                        // Use guest orders as fallback
                        orders = guestOrders;
                    }
                    else
                    {
                        Console.WriteLine("No orders found in the system at all. Check database connection.");
                    }
                }
                
                // Get total count for pagination metadata
                var totalCount = orders.Count();
                
                Console.WriteLine($"Found {totalCount} orders matching criteria");
                
                // Create pagination metadata
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                var paginationMetadata = new
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    HasPrevious = pageNumber > 1,
                    HasNext = pageNumber < totalPages
                };
                
                // Add pagination metadata to response headers
                Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));
                
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all orders: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
