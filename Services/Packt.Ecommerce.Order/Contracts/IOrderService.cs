using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Packt.Ecommerce.DTO.Models;

namespace Packt.Ecommerce.Order.Contracts
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDetailsViewModel>> GetOrdersAsync(string filterCriteria = null);

        Task<OrderDetailsViewModel> GetOrderByIdAsync(string orderId);

        Task<OrderDetailsViewModel> AddOrderAsync(OrderDetailsViewModel order);

        Task<HttpResponseMessage> UpdateOrderAsync(OrderDetailsViewModel order);
    }
}
