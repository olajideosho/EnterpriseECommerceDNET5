using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packt.Ecommerce.DTO.Models;

namespace Packt.Ecommerce.Web.Contracts
{
    public interface IECommerceService
    {
        Task<IEnumerable<ProductListViewModel>> GetProductsAsync(string filterCriteria = null);

        Task<ProductDetailsViewModel> GetProductByIdAsync(string productId, string productName);

        Task<OrderDetailsViewModel> CreateOrUpdateOrder(OrderDetailsViewModel order);

        Task<OrderDetailsViewModel> GetOrderByIdAsync(string orderId);

        Task<InvoiceDetailsViewModel> GetInvoiceByIdAsync(string invoiceId);

        Task<InvoiceDetailsViewModel> SubmitOrder(OrderDetailsViewModel order);
    }
}
