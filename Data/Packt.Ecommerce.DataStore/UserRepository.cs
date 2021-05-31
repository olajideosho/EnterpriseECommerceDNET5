using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Packt.Ecommerce.DataStore.Contracts;

namespace Packt.Ecommerce.DataStore
{
    public class UserRepository : BaseRepository<Data.Models.User>, IUserRepository
    {

        private readonly IOptions<DatabaseSettingsOptions> databaseSettings;

        public UserRepository(CosmosClient cosmosClient, IOptions<DatabaseSettingsOptions> databaseSettingsOption) : base(cosmosClient, databaseSettingsOption?.Value.DataBaseName, "Users")
        {
            this.databaseSettings = databaseSettingsOption;
        }
    }
}
