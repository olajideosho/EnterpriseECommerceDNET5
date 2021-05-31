using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Packt.Ecommerce.Caching.Interfaces;
using Packt.Ecommerce.Common.Validator;

namespace Packt.Ecommerce.Caching
{
    [ExcludeFromCodeCoverage]
    public class EntitySerializer : IEntitySerializer
    {
        public async Task<T> DeserializeEntityAsync<T>(byte[] entity, CancellationToken cancellationToken = default)
        {
            NotNullValidator.ThrowIfNull(entity, nameof(entity));

            using MemoryStream memoryStream = new MemoryStream(entity);
            var value = await JsonSerializer.DeserializeAsync<T>(memoryStream, cancellationToken: cancellationToken);
            return value;
        }

        public async Task<byte[]> SerializeEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
        {
            using MemoryStream memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, entity, cancellationToken: cancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream.ToArray();
        }
    }
}
