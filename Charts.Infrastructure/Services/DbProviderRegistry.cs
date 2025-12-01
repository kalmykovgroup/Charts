using System.Data.Common;
using Charts.Domain.Interfaces;

namespace Charts.Infrastructure.Services
{
    public sealed class DbProviderRegistry : IDbProviderRegistry
    {
        private readonly IReadOnlyDictionary<string, DbProviderFactory> _map;

        public DbProviderRegistry(IReadOnlyDictionary<string, DbProviderFactory> map)
            => _map = map;

        public DbProviderFactory GetFactory(string provider)
            => _map.TryGetValue(provider, out var f)
               ? f
               : throw new InvalidOperationException($"Unknown ADO.NET provider '{provider}'.");
    }
}
