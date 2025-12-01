using System.Data.Common;
using Charts.Domain.Contracts.Types;

namespace Charts.Domain.Contracts
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
