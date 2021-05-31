using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Packt.Ecommerce.DTO.Models;

namespace Packt.Ecommerce.Product.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListViewModel>> GetProductsAsync(string filterCriteria = null);

        Task<ProductDetailsViewModel> GetProductByIdAsync(string productId, string productName);

        Task<ProductDetailsViewModel> AddProductAsync(ProductDetailsViewModel product);

        Task<HttpResponseMessage> UpdateProductAsync(ProductDetailsViewModel product);

        Task<HttpResponseMessage> DeleteProductAsync(string productId, string productName);
    }
}
