using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Packt.Ecommerce.Data.Models;
using Packt.Ecommerce.DataStore.Contracts;

namespace Packt.Ecommerce.DataStore
{
    public class OrderRepository: BaseRepository<Order>, IOrderRepository
    {
        private readonly IOptions<DatabaseSettingsOptions> databaseSettings;

        public OrderRepository(CosmosClient cosmosClient, IOptions<DatabaseSettingsOptions> databaseSettingsOption) : base(cosmosClient, databaseSettingsOption?.Value.DataBaseName, "Orders")
        {
            this.databaseSettings = databaseSettingsOption;
        }
    }
}
