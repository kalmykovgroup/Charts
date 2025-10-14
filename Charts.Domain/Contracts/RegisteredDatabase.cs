

using Charts.Api.Domain.Contracts.Types;
using System.Data.Common;

namespace Charts.Api.Application.Contracts
{
    public sealed record RegisteredDatabase
    {
        public Guid Id { get; init; }
        public string Key { get; init; } = "";             // человекочитаемое имя/ключ
        public string DatabaseVersion { get; init; } = "";
        public string ConnectionString { get; init; } = "";
        public DbProviderType Provider { get; init; } = DbProviderType.Unknown;

        // Провайдер-специфичный «сырой» DataSource (например, NpgsqlDataSource).
        public object? RawDataSource { get; init; }

        // Унифицированный способ открыть соединение:
        public required Func<CancellationToken, Task<DbConnection>> OpenConnectionAsync { get; init; }
    }

}
