using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Packt.Ecommerce.Data.Models;
using Packt.Ecommerce.DataStore.Contracts;

namespace Packt.Ecommerce.DataAccess.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository repository;

        public OrdersController(IOrderRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAordersAsync(string filterCriteria = null)
        {
            IEnumerable<Order> order;
            if (string.IsNullOrEmpty(filterCriteria))
            {
                order = await this.repository.GetAsync(string.Empty).ConfigureAwait(false);
            }
            else
            {
                order = await this.repository.GetAsync(filterCriteria).ConfigureAwait(false);
            }

            if (order.Any())
            {
                return this.Ok(order);
            }
            else
            {
                return this.NoContent();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            Order result = await this.repository.GetByIdAsync(id, id).ConfigureAwait(false);
            if(result != null)
            {
                return this.Ok(result);
            }
            else
            {
                return this.NoContent();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] Order order)
        {
            if(order == null || order.Etag != null)
            {
                return this.BadRequest();
            }

            var result = await this.repository.AddAsync(order, order.Id).ConfigureAwait(false);
            return this.CreatedAtAction(nameof(this.GetOrderById), new { Id = result.Resource.Id }, result.Resource);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderAsync([FromBody] Order order)
        {
            if(order == null || order.Etag == null || order.Id == null)
            {
                return this.BadRequest();
            }

            bool result = await this.repository.ModifyAsync(order, order.Etag, order.Id).ConfigureAwait(false);
            if (result)
            {
                return this.Accepted();
            }
            else
            {
                return this.NoContent();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderAsync(string id)
        {
            bool result = await this.repository.RemoveAsync(id, id).ConfigureAwait(false);

            if (result)
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
