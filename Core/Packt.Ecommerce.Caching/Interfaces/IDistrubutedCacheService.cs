using System;
using System.Threading;
using System.Threading.Tasks;

namespace Packt.Ecommerce.Caching.Interfaces
{
    public interface IDistributedCacheService
    {
        Task AddOrUpdateCacheAsync<T>(string cacheEntityKey, T cacheEntity, TimeSpan? absoluteExpiration = default, CancellationToken cancellationToken = default);

        Task AddOrUpdateCacheStringAsync(string cacheEntityKey, string cacheEntity, TimeSpan? absoluteExpiration = default, CancellationToken cancellationToken = default);

        Task<T> GetCacheAsync<T>(string cacheEntityKey, CancellationToken cancellationToken = default);

        Task<string> GetCacheStringAsync(string cacheEntityKey, CancellationToken cancellationToken = default);

        Task RefreshCacheAsync(string cacheEntityKey, CancellationToken cancellationToken = default);

        Task RemoveCacheAsync(string cacheEntityKey, CancellationToken cancellationToken = default);
    }
}
