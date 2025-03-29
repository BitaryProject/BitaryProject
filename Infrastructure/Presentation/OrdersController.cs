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

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]

    public class OrdersController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderResult>> Create([FromBody] OrderRequest request)
        {
            Console.WriteLine("Request reached Create method");
            var email = User.FindFirstValue(ClaimTypes.Email);
            var order = await serviceManager.OrderService.CreateOrUpdateOrderAsync(request,  email);

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResult>>> GetOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var orders = await serviceManager.OrderService.GetOrderByEmailAsync(email);

            return Ok(orders);
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

    }

}
