using System.Data.Common;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Interfaces;

namespace Charts.Infrastructure.Databases
{
    public sealed class CurrentDb : ICurrentDb
    {
        private readonly IRequestDbKeyAccessor _key;
        private readonly IDatabaseRegistry _reg;

        public CurrentDb(IRequestDbKeyAccessor key, IDatabaseRegistry reg)
        {
            _key = key; _reg = reg;
        }

        private RegisteredDatabase Current => _key.DbId is { } id ? _reg.ResolveById(id) : throw new InvalidOperationException("No database selected");

        public Guid? Key => _key.DbId; 
        public DbProviderType Provider => Current.Provider;

        public async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken ct = default)
            => await Current.OpenConnectionAsync(ct);
    }
}
