using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Packt.Ecommerce.Caching.Interfaces;

namespace Packt.Ecommerce.Caching
{
    public class DistributedCacheService : IDistributedCacheService
    {
        private const long DefaultCacheAbsoluteExpirationMinutes = 60;

        private readonly IEntitySerializer entitySerializer;

        private readonly IDistributedCache distributedCache;

        private readonly DistributedCacheEntryOptions distributedCacheEntryOptions;

        private readonly TimeSpan defaultCacheEntryAbsoluteExpirationTime;

        public DistributedCacheService(IDistributedCache distributedCache, IEntitySerializer entitySerializer)
        {
            this.distributedCache = distributedCache;
            this.entitySerializer = entitySerializer;
            this.defaultCacheEntryAbsoluteExpirationTime = TimeSpan.FromMinutes(DefaultCacheAbsoluteExpirationMinutes);
            this.distributedCacheEntryOptions = new DistributedCacheEntryOptions();
        }

        public async Task AddOrUpdateCacheAsync<T>(string cacheEntityKey, T cacheEntity, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            var absoluteExpiryTime = absoluteExpiration == null
                                    ? this.defaultCacheEntryAbsoluteExpirationTime
                                    : TimeSpan.FromSeconds(absoluteExpiration.Value.TotalSeconds);
            var byteValue = await this.entitySerializer.SerializeEntityAsync<T>(cacheEntity, cancellationToken).ConfigureAwait(false);
            await this.distributedCache.SetAsync(cacheEntityKey, byteValue, this.distributedCacheEntryOptions.SetAbsoluteExpiration(absoluteExpiryTime), cancellationToken).ConfigureAwait(false);
        }

        public async Task AddOrUpdateCacheStringAsync(string cacheEntityKey, string cacheEntity, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            var absoluteExpiryTime = absoluteExpiration == null
                                   ? this.defaultCacheEntryAbsoluteExpirationTime
                                   : TimeSpan.FromSeconds(absoluteExpiration.Value.TotalSeconds);
            await this.distributedCache.SetStringAsync(cacheEntityKey, cacheEntity, this.distributedCacheEntryOptions.SetAbsoluteExpiration(absoluteExpiryTime), cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> GetCacheAsync<T>(string cacheEntityKey, CancellationToken cancellationToken = default)
        {
            var obj = await this.distributedCache.GetAsync(cacheEntityKey, cancellationToken).ConfigureAwait(false);
            return obj != null ? await this.entitySerializer.DeserializeEntityAsync<T>(obj) : default;
        }

        public async Task<string> GetCacheStringAsync(string cacheEntityKey, CancellationToken cancellationToken = default)
        {
            return await this.distributedCache.GetStringAsync(cacheEntityKey, cancellationToken).ConfigureAwait(false);
        }

        public async Task RefreshCacheAsync(string cacheEntityKey, CancellationToken cancellationToken = default)
        {
            await this.distributedCache.RefreshAsync(cacheEntityKey, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveCacheAsync(string cacheEntityKey, CancellationToken cancellationToken = default)
        {
            await this.distributedCache.RemoveAsync(cacheEntityKey, cancellationToken).ConfigureAwait(false);
        }
    }
}
