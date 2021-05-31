using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Packt.Ecommerce.DataStore.Contracts
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync(string filterCriteria);

        Task<TEntity> GetByIdAsync(string id, string partitionKey);

        Task<ItemResponse<TEntity>> AddAsync(TEntity entity, string partitionKey);

        Task<bool> ModifyAsync(TEntity entity, string etag, string partitionKey);

        Task<bool> RemoveAsync(string id, string partitionKey);
    }
}
