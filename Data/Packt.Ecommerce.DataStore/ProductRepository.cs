using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Packt.Ecommerce.Data.Models;
using Packt.Ecommerce.DataStore.Contracts;

namespace Packt.Ecommerce.DataStore
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly IOptions<DatabaseSettingsOptions> databaseSettings;

        public ProductRepository(CosmosClient cosmosClient, IOptions<DatabaseSettingsOptions> databaseSetttingsOption) : base(cosmosClient, databaseSetttingsOption?.Value.DataBaseName, "Products")
        {
            this.databaseSettings = databaseSetttingsOption;
        }
    }
}
