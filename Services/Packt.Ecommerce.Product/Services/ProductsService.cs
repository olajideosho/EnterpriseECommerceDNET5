using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Packt.Ecommerce.Caching.Interfaces;
using Packt.Ecommerce.Common.Models;
using Packt.Ecommerce.Common.Options;
using Packt.Ecommerce.Common.Validator;
using Packt.Ecommerce.DTO.Models;
using Packt.Ecommerce.Product.Contracts;

namespace Packt.Ecommerce.Product.Services
{
    public class ProductsService : IProductService
    {
        private const string ContentType = "application/json";

        private readonly IOptions<ApplicationSettings> applicationSettings;

        private readonly HttpClient httpClient;

        private readonly IMapper autoMapper;

        private readonly IDistributedCacheService cacheService;

        public ProductsService(IHttpClientFactory httpClientFactory, IOptions<ApplicationSettings> applicationSettings, IMapper autoMapper, IDistributedCacheService cacheService)
        {
            NotNullValidator.ThrowIfNull(applicationSettings, nameof(applicationSettings));
            IHttpClientFactory httpclientFactory = httpClientFactory;
            this.applicationSettings = applicationSettings;
            this.httpClient = httpclientFactory.CreateClient();
            this.autoMapper = autoMapper;
            this.cacheService = cacheService;
        }

        public async Task<ProductDetailsViewModel> AddProductAsync(ProductDetailsViewModel product)
        {
            NotNullValidator.ThrowIfNull(product, nameof(product));
            product.CreatedDate = DateTime.UtcNow;
            using var productRequest = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, ContentType);
            var productResponse = await this.httpClient.PostAsync(new Uri($"{this.applicationSettings.Value.DataStoreEndpoint}api/products"), productRequest).ConfigureAwait(false);

            if (!productResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
            }

            var createdProductDAO = await productResponse.Content.ReadFromJsonAsync<Data.Models.Product>().ConfigureAwait(false);

            await this.cacheService.RemoveCacheAsync("products").ConfigureAwait(false);

            var createdProduct = this.autoMapper.Map<ProductDetailsViewModel>(createdProductDAO);
            return createdProduct;
        }

        public async Task<HttpResponseMessage> DeleteProductAsync(string productId, string productName)
        {
            var productResponse = await this.httpClient.DeleteAsync(new Uri($"{this.applicationSettings.Value.DataStoreEndpoint}api/products/{productId}?name={productName}")).ConfigureAwait(false);

            if (!productResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
            }

            await this.cacheService.RemoveCacheAsync("products").ConfigureAwait(false);

            return productResponse;
        }

        public async Task<ProductDetailsViewModel> GetProductByIdAsync(string productId, string productName)
        {
            using var productRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.Value.DataStoreEndpoint}api/products/{productId}?name={productName}");
            var productResponse = await this.httpClient.SendAsync(productRequest).ConfigureAwait(false);
            if (!productResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
            }

            if (productResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var productDAO = await productResponse.Content.ReadFromJsonAsync<Packt.Ecommerce.Data.Models.Product>().ConfigureAwait(false);

                var product = this.autoMapper.Map<ProductDetailsViewModel>(productDAO);
                return product;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<ProductListViewModel>> GetProductsAsync(string filterCriteria = null)
        {
            var products = await this.cacheService.GetCacheAsync<IEnumerable<Data.Models.Product>>($"products{filterCriteria}").ConfigureAwait(false);

            if (products == null)
            {
                using var productRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.Value.DataStoreEndpoint}api/products?filterCriteria={filterCriteria}");
                var productResponse = await this.httpClient.SendAsync(productRequest).ConfigureAwait(false);

                if (!productResponse.IsSuccessStatusCode)
                {
                    await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
                }

                if (productResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return new List<ProductListViewModel>();
                }

                products = await productResponse.Content.ReadFromJsonAsync<IEnumerable<Data.Models.Product>>().ConfigureAwait(false);
                await this.cacheService.AddOrUpdateCacheAsync<IEnumerable<Packt.Ecommerce.Data.Models.Product>>($"products{filterCriteria}", products).ConfigureAwait(false);
            }

            var productList = this.autoMapper.Map<List<ProductListViewModel>>(products);
            return productList;
        }

        public async Task<HttpResponseMessage> UpdateProductAsync(ProductDetailsViewModel product)
        {
            using var productRequest = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, ContentType);
            var productResponse = await this.httpClient.PutAsync(new Uri($"{this.applicationSettings.Value.DataStoreEndpoint}api/products"), productRequest).ConfigureAwait(false);
            if (!productResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
            }

            await this.cacheService.RemoveCacheAsync("products").ConfigureAwait(false);

            return productResponse;
        }

        private async Task ThrowServiceToServiceErrors(HttpResponseMessage response)
        {
            var exceptionReponse = await response.Content.ReadFromJsonAsync<ExceptionResponse>().ConfigureAwait(false);
            throw new Exception(exceptionReponse.InnerException);
        }
    }
}
