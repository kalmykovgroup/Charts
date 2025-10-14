using Charts.Api.Application.Contracts;
using Charts.Api.Application.Interfaces;
using Charts.Api.Domain.Contracts.Types;
using System.Data.Common;

namespace Charts.Api.Infrastructure.Databases
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
