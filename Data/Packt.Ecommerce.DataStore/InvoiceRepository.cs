using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Packt.Ecommerce.Data.Models;
using Packt.Ecommerce.DataStore.Contracts;

namespace Packt.Ecommerce.DataStore
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        private readonly IOptions<DatabaseSettingsOptions> databaseSettings;

        public InvoiceRepository(CosmosClient cosmosClient, IOptions<DatabaseSettingsOptions> databaseSettingsOption) : base(cosmosClient, databaseSettingsOption?.Value.DataBaseName, "Invoices")
        {
            this.databaseSettings = databaseSettingsOption;
        }
    }
}
