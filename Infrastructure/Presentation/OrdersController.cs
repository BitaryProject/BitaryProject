using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities.OrderEntities;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ApiController
    {
        private readonly IServiceManager serviceManager;

        public OrdersController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

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

        [HttpGet]
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
                
                // Log the claims to help diagnose authentication issues
                Console.WriteLine("User claims:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }
                
                var orders = await serviceManager.OrderService.GetOrderByEmailAsync(email);
                
                Console.WriteLine($"Initial order count: {orders.Count()}");
                
                // Format the orders for better display
                var formattedOrders = orders.Select(o => new {
                    id = o.Id,
                    orderDate = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    status = o.PaymentStatus,
                    deliveryMethod = o.DeliveryMethod,
                    subtotal = o.Subtotal,
                    total = o.Total,
                    orderItems = o.OrderItems.Select(item => new {
                        productName = item.ProductName,
                        quantity = item.Quantity,
                        price = item.Price,
                        pictureUrl = item.PictureUrl
                    }).ToList(),
                    shippingAddress = o.ShippingAddress != null ? new {
                        name = o.ShippingAddress.Name,
                        street = o.ShippingAddress.Street,
                        city = o.ShippingAddress.City,
                        country = o.ShippingAddress.Country
                    } : null
                }).ToList();
                
                return Ok(formattedOrders);
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
        /// Admin endpoint to get all orders with filtering and pagination
        /// </summary>
        [HttpGet("admin/all-orders")]
        [Authorize(Roles = "Admin")] // Restrict to admin role
        public async Task<ActionResult<IEnumerable<OrderResult>>> GetAllOrdersAdmin(
            [FromQuery] string email = null,
            [FromQuery] string status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                Console.WriteLine($"Admin request for orders - Email filter: {email ?? "None"}, Status: {status ?? "None"}");
                
                IEnumerable<OrderResult> orders;
                
                // If email is provided, get orders for that specific user
                if (!string.IsNullOrEmpty(email))
                {
                    orders = await serviceManager.OrderService.GetOrderByEmailAsync(email);
                    Console.WriteLine($"Found {orders.Count()} orders for email {email}");
                }
                else
                {
                    // Otherwise get all orders with optional status filter
                    orders = await serviceManager.OrderService.GetAllOrdersAsync(status, pageNumber, pageSize);
                    Console.WriteLine($"Found {orders.Count()} orders with status filter: {status ?? "None"}");
                }
                
                // Format orders for better display
                var formattedOrders = orders.Select(o => new {
                    id = o.Id,
                    userEmail = o.UserEmail,
                    orderDate = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    status = o.PaymentStatus,
                    deliveryMethod = o.DeliveryMethod,
                    subtotal = o.Subtotal,
                    total = o.Total,
                    itemCount = o.OrderItems?.Count() ?? 0,
                    paymentMethod = o.PaymentMethod,
                    shippingAddress = o.ShippingAddress != null ? new {
                        name = o.ShippingAddress.Name,
                        street = o.ShippingAddress.Street,
                        city = o.ShippingAddress.City,
                        country = o.ShippingAddress.Country
                    } : null
                }).ToList();
                
                // Create pagination metadata
                var totalCount = formattedOrders.Count;
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
                
                return Ok(new {
                    orders = formattedOrders,
                    pagination = paginationMetadata
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllOrdersAdmin: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message });
            }
        }
    }
} 