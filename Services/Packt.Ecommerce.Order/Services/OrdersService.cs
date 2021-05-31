using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
using Packt.Ecommerce.Order.Contracts;

namespace Packt.Ecommerce.Order.Services
{
    public class OrdersService : IOrderService
    {
        private const string ContentType = "application/json";

        private readonly IOptions<ApplicationSettings> applicationSettings;

        private readonly HttpClient httpClient;

        private readonly IMapper autoMapper;

        private readonly IDistributedCacheService cacheService;

        public OrdersService(IHttpClientFactory httpClientFactory, IOptions<ApplicationSettings> applicationSettings, IMapper autoMapper, IDistributedCacheService cacheService)
        {
            NotNullValidator.ThrowIfNull(applicationSettings, nameof(applicationSettings));
            IHttpClientFactory httpclientFactory = httpClientFactory;
            this.applicationSettings = applicationSettings;
            this.httpClient = httpclientFactory.CreateClient();
            this.autoMapper = autoMapper;
            this.cacheService = cacheService;
        }

        public async Task<OrderDetailsViewModel> AddOrderAsync(OrderDetailsViewModel order)
        {
            NotNullValidator.ThrowIfNull(order, nameof(order));

            // Order entity is used for Shopping cart and at any point as there can only be one shopping cart, checking for existing shopping cart
            var getExistingOrder = await this.GetOrdersAsync($" e.UserId = '{order.UserId}' and e.OrderStatus = '{OrderStatus.Cart}' ").ConfigureAwait(false);
            OrderDetailsViewModel existingOrder = getExistingOrder.FirstOrDefault();
            if (existingOrder != null)
            {
                order.Id = existingOrder.Id;
                order.Etag = existingOrder.Etag;
                if (order.OrderStatus == OrderStatus.Submitted.ToString())
                {
                    order.OrderPlacedDate = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                    order.DeliveryDate = DateTime.UtcNow.AddDays(5).ToString(CultureInfo.InvariantCulture);
                    Random trackingId = new Random();
                    order.TrackingId = trackingId.Next(int.MaxValue); // generating random tracking number
                }
                else
                {
                    order.Products.AddRange(existingOrder.Products); // For cart append products
                    order.OrderStatus = OrderStatus.Cart.ToString();
                }

                order.OrderTotal = order.Products.Sum(x => x.Price);
                await this.UpdateOrderAsync(order).ConfigureAwait(false);
                return order;
            }
            else
            {
                order.OrderStatus = OrderStatus.Cart.ToString();
                order.Id = Guid.NewGuid().ToString();
                order.OrderTotal = order.Products.Sum(x => x.Price);
                using var orderRequest = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, ContentType);
                var orderResponse = await this.httpClient.PostAsync(new Uri($"{this.applicationSettings.Value.DataStoreEndpoint}api/orders"), orderRequest).ConfigureAwait(false);

                if (!orderResponse.IsSuccessStatusCode)
                {
                    await this.ThrowServiceToServiceErrors(orderResponse).ConfigureAwait(false);
                }

                var createdOrderDAO = await orderResponse.Content.ReadFromJsonAsync<Packt.Ecommerce.Data.Models.Order>().ConfigureAwait(false);

                var createdOrder = this.autoMapper.Map<OrderDetailsViewModel>(createdOrderDAO);
                return createdOrder;
            }
        }

        public async Task<OrderDetailsViewModel> GetOrderByIdAsync(string orderId)
        {
            using var orderRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.Value.DataStoreEndpoint}api/orders/{orderId}");
            var orderResponse = await this.httpClient.SendAsync(orderRequest).ConfigureAwait(false);
            if (!orderResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(orderResponse).ConfigureAwait(false);
            }

            if (orderResponse.StatusCode != HttpStatusCode.NoContent)
            {
                var orderDAO = await orderResponse.Content.ReadFromJsonAsync<Packt.Ecommerce.Data.Models.Order>().ConfigureAwait(false);

                var order = this.autoMapper.Map<OrderDetailsViewModel>(orderDAO);
                return order;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<OrderDetailsViewModel>> GetOrdersAsync(string filterCriteria = null)
        {
            using var orderRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.Value.DataStoreEndpoint}api/orders?filterCriteria={filterCriteria}");
            var orderResponse = await this.httpClient.SendAsync(orderRequest).ConfigureAwait(false);

            if (!orderResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(orderResponse).ConfigureAwait(false);
            }

            if (orderResponse.StatusCode != HttpStatusCode.NoContent)
            {
                var orders = await orderResponse.Content.ReadFromJsonAsync<IEnumerable<Packt.Ecommerce.Data.Models.Order>>().ConfigureAwait(false);

                var orderList = this.autoMapper.Map<List<OrderDetailsViewModel>>(orders);
                return orderList;
            }
            else
            {
                return new List<OrderDetailsViewModel>();
            }
        }

        public async Task<HttpResponseMessage> UpdateOrderAsync(OrderDetailsViewModel order)
        {
            using var orderRequest = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, ContentType);
            var orderResponse = await this.httpClient.PutAsync(new Uri($"{this.applicationSettings.Value.DataStoreEndpoint}api/orders"), orderRequest).ConfigureAwait(false);
            if (!orderResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(orderResponse).ConfigureAwait(false);
            }

            return orderResponse;
        }

        private async Task ThrowServiceToServiceErrors(HttpResponseMessage response)
        {
            var exceptionReponse = await response.Content.ReadFromJsonAsync<ExceptionResponse>().ConfigureAwait(false);
            throw new Exception(exceptionReponse.InnerException);
        }
    }
}
