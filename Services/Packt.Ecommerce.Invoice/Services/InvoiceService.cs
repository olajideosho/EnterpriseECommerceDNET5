using System;
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
using Packt.Ecommerce.Invoice.Contracts;

namespace Packt.Ecommerce.Invoice.Services
{
    public class InvoiceService : IInvoiceService
    {
        private const string ContentType = "application/json";

        private readonly IOptions<ApplicationSettings> applicationSettings;

        private readonly HttpClient httpClient;

        private readonly IMapper autoMapper;

        private readonly IDistributedCacheService cacheService;

        public InvoiceService(IHttpClientFactory httpClientFactory, IOptions<ApplicationSettings> applicationSettings, IMapper autoMapper, IDistributedCacheService cacheService)
        {
            NotNullValidator.ThrowIfNull(applicationSettings, nameof(applicationSettings));
            IHttpClientFactory httpclientFactory = httpClientFactory;
            this.applicationSettings = applicationSettings;
            this.httpClient = httpclientFactory.CreateClient();
            this.autoMapper = autoMapper;
            this.cacheService = cacheService;
        }

        public async Task<InvoiceDetailsViewModel> AddInvoiceAsync(InvoiceDetailsViewModel invoice)
        {
            NotNullValidator.ThrowIfNull(invoice, nameof(invoice));
            invoice.Id = Guid.NewGuid().ToString();
            invoice.SoldBy = new SoldByViewModel()
            {
                Email = "Packt@Packt.com",
                Phone = "9876543210",
                SellerName = "Packt",
            };
            using var invoiceRequest = new StringContent(JsonSerializer.Serialize(invoice), Encoding.UTF8, ContentType);
            var invoiceResponse = await this.httpClient.PostAsync(new Uri($"{this.applicationSettings.Value.DataStoreEndpoint}api/invoice"), invoiceRequest).ConfigureAwait(false);

            if (!invoiceResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(invoiceResponse).ConfigureAwait(false);
            }

            var createdInvoiceDAO = await invoiceResponse.Content.ReadFromJsonAsync<Packt.Ecommerce.Data.Models.Invoice>().ConfigureAwait(false);

            var createdInvoice = this.autoMapper.Map<InvoiceDetailsViewModel>(createdInvoiceDAO);
            return createdInvoice;
        }

        public async Task<InvoiceDetailsViewModel> GetInvoiceByIdAsync(string invoiceId)
        {
            using var invoiceRequest = new HttpRequestMessage(HttpMethod.Get, $"{this.applicationSettings.Value.DataStoreEndpoint}api/invoice/{invoiceId}");
            var invoiceResponse = await this.httpClient.SendAsync(invoiceRequest).ConfigureAwait(false);
            if (!invoiceResponse.IsSuccessStatusCode)
            {
                await this.ThrowServiceToServiceErrors(invoiceResponse).ConfigureAwait(false);
            }

            if (invoiceResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var invoiceDAO = await invoiceResponse.Content.ReadFromJsonAsync<Packt.Ecommerce.Data.Models.Invoice>().ConfigureAwait(false);

                var invoice = this.autoMapper.Map<InvoiceDetailsViewModel>(invoiceDAO);
                return invoice;
            }
            else
            {
                return null;
            }
        }

        private async Task ThrowServiceToServiceErrors(HttpResponseMessage response)
        {
            var exceptionReponse = await response.Content.ReadFromJsonAsync<ExceptionResponse>().ConfigureAwait(false);
            throw new Exception(exceptionReponse.InnerException);
        }
    }
}
