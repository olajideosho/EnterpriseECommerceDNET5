using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Packt.Ecommerce.DTO.Models;
using Packt.Ecommerce.Order.Contracts;

namespace Packt.Ecommerce.Order.Controllers
{
    [Route("api/Orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] string filterCriteria = null)
        {
            var orders = await this.orderService.GetOrdersAsync(filterCriteria).ConfigureAwait(false);
            if (orders.Any())
            {
                return this.Ok(orders);
            }
            else
            {
                return this.NoContent();
            }
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await this.orderService.GetOrderByIdAsync(id).ConfigureAwait(false);
            if (order != null)
            {
                return this.Ok(order);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderAsync(OrderDetailsViewModel order)
        {
            // Order null check is to avoid null attribute validation error.
            ////if (order == null || order.Etag != null)
            ////{
            ////    return this.BadRequest();
            ////}

            var result = await this.orderService.AddOrderAsync(order).ConfigureAwait(false);
            return this.CreatedAtAction(nameof(this.GetOrderById), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderAsync(OrderDetailsViewModel order)
        {
            if (order == null || order.Etag == null || order.Id == null)
            {
                return this.BadRequest();
            }

            var result = await this.orderService.UpdateOrderAsync(order).ConfigureAwait(false);
            if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return this.Accepted();
            }
            else
            {
                return this.NoContent();
            }
        }
    }
}
