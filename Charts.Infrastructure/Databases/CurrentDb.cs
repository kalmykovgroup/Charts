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
        private RegisteredDatabase? _cached;

        public CurrentDb(IRequestDbKeyAccessor key, IDatabaseRegistry reg)
        {
            _key = key;
            _reg = reg;
        }

        public Guid? Key => _key.DbId;

        public DbProviderType Provider
        {
            get
            {
                if (_cached != null)
                    return _cached.Provider;

                if (_key.DbId is not { } id)
                    throw new InvalidOperationException("No database selected");

                // Синхронный fallback для свойства
                if (_reg.TryResolveById(id, out var db))
                {
                    _cached = db;
                    return db.Provider;
                }

                throw new InvalidOperationException($"Database with id '{id}' not found. Use OpenConnectionAsync first.");
            }
        }

        public async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken ct = default)
        {
            if (_key.DbId is not { } id)
                throw new InvalidOperationException("No database selected");

            // Используем асинхронный метод с ленивой загрузкой
            _cached ??= await _reg.ResolveByIdAsync(id, ct);
            return await _cached.OpenConnectionAsync(ct);
        }
    }
}
