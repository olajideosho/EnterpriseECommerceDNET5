using System;
using System.Threading;
using System.Threading.Tasks;

namespace Packt.Ecommerce.Caching.Interfaces
{
    public interface IEntitySerializer
    {
        Task<byte[]> SerializeEntityAsync<T>(T entity, CancellationToken cancellationToken = default);

        Task<T> DeserializeEntityAsync<T>(byte[] entity, CancellationToken cancellationToken = default);
    }
}
