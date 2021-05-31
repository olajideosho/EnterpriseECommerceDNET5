using System;
namespace Packt.Ecommerce.Common.Options
{
    public class ApplicationSettings
    {
        public bool IncludeExceptionStackInResponse { get; set; }

        public bool UseRedisCache { get; set; }

        public string DataStoreEndpoint { get; set; }

        public string ProductsApiEndpoint { get; set; }

        public string InvoiceApiEndpoint { get; set; }

        public string OrdersApiEndpoint { get; set; }
    }
}
