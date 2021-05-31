using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Packt.Ecommerce.Common.Models;
using Packt.Ecommerce.Common.Options;
using Packt.Ecommerce.Common.Validator;
using Packt.Ecommerce.DTO.Models;
using Packt.Ecommerce.Web.Contracts;

namespace Packt.Ecommerce.Web.Services
{
    public class ECommerceService : IECommerceService
    {
        private const string ContentType = "application/json";

        private readonly HttpClient httpClient;

        private readonly ApplicationSettings applicationSettings;

        public ECommerceService(IHttpClientFactory httpClientFactory, IOptions<ApplicationSettings> applicationSettings)
        {
            NotNullValidator.ThrowIfNull(applicationSettings, nameof(applicationSettings));
            IHttpClientFactory httpclientFactory = httpClientFactory;
            this.httpClient = httpclientFactory.CreateClient();
            this.applicationSettings = applicationSettings.Value;
        }


        public async Task<OrderDetailsViewModel> CreateOrUpdateOrder(OrderDetailsViewModel order)
        {
            NotNullValidator.ThrowIfNull(order, nameof(order));
            using var orderRequest = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, ContentType);
            var orderResponse = await this.httpClient.PostAsync(new Uri($"{this.applicationSettings.OrdersApiEndpoint}"), orderRequest).ConfigureAwait(false);

            if (!orderResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(orderResponse).ConfigureAwait(false);
            }

            var createdOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDetailsViewModel>().ConfigureAwait(false);

            return createdOrder;
        }

        public async Task<InvoiceDetailsViewModel> GetInvoiceByIdAsync(string invoiceId)
        {
            InvoiceDetailsViewModel invoice = new InvoiceDetailsViewModel();
            using var invoiceRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.InvoiceApiEndpoint}{invoiceId}");
            var invoiceResponse = await this.httpClient.SendAsync(invoiceRequest).ConfigureAwait(false);
            if (!invoiceResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(invoiceResponse).ConfigureAwait(false);
            }

            if (invoiceResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                invoice = await invoiceResponse.Content.ReadFromJsonAsync<InvoiceDetailsViewModel>().ConfigureAwait(false);
            }

            return invoice;
        }

        public async Task<OrderDetailsViewModel> GetOrderByIdAsync(string orderId)
        {
            OrderDetailsViewModel order = new OrderDetailsViewModel();
            using var orderRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.OrdersApiEndpoint}{orderId}");
            var productResponse = await this.httpClient.SendAsync(orderRequest).ConfigureAwait(false);
            if (!productResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
            }

            if (productResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                order = await productResponse.Content.ReadFromJsonAsync<OrderDetailsViewModel>().ConfigureAwait(false);
            }

            return order;
        }

        public async Task<ProductDetailsViewModel> GetProductByIdAsync(string productId, string productName)
        {
            ProductDetailsViewModel product = new ProductDetailsViewModel();
            using var productRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.ProductsApiEndpoint}{productId}?name={productName}");
            var productResponse = await this.httpClient.SendAsync(productRequest).ConfigureAwait(false);
            if (!productResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
            }

            if (productResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                product = await productResponse.Content.ReadFromJsonAsync<ProductDetailsViewModel>().ConfigureAwait(false);
            }

            return product;
        }

        public async Task<IEnumerable<ProductListViewModel>> GetProductsAsync(string filterCriteria = null)
        {
            IEnumerable<ProductListViewModel> products = new List<ProductListViewModel>();
            using var productRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.ProductsApiEndpoint}?filterCriteria={filterCriteria}");
            var productResponse = await this.httpClient.SendAsync(productRequest).ConfigureAwait(false);

            if (!productResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(productResponse).ConfigureAwait(false);
            }

            if (productResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                products = await productResponse.Content.ReadFromJsonAsync<IEnumerable<ProductListViewModel>>().ConfigureAwait(false);
            }

            if (products.Any())
            {
                throw new InvalidOperationException();
            }

            return products;
        }

        public async Task<InvoiceDetailsViewModel> SubmitOrder(OrderDetailsViewModel order)
        {
            order.OrderStatus = OrderStatus.Submitted.ToString();
            await this.CreateOrUpdateOrder(order).ConfigureAwait(false);

            InvoiceDetailsViewModel invoice = new InvoiceDetailsViewModel()
            {
                OrderId = order.Id,
                PaymentMode = order.PaymentMode,
                Products = order.Products,
                ShippingAddress = order.ShippingAddress,
            };
            using var invoiceRequest = new StringContent(JsonSerializer.Serialize(invoice), Encoding.UTF8, ContentType);
            var invoiceResponse = await this.httpClient.PostAsync(new Uri($"{this.applicationSettings.InvoiceApiEndpoint}"), invoiceRequest).ConfigureAwait(false);
            if (!invoiceResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(invoiceResponse).ConfigureAwait(false);
            }

            invoice = await invoiceResponse.Content.ReadFromJsonAsync<InvoiceDetailsViewModel>().ConfigureAwait(false);

            return invoice;
        }

        private async Task ThrowServiceToServiceErrors(HttpResponseMessage response)
        {
            var exceptionReponse = await response.Content.ReadFromJsonAsync<ExceptionResponse>().ConfigureAwait(false);
            throw new Exception(exceptionReponse.InnerException);
        }
    }
}
