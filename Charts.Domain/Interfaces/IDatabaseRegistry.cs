using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Types;

namespace Charts.Domain.Interfaces
{
    public interface IDatabaseRegistry
    {
        Task InitializeAsync(CancellationToken ct = default);
        Task ReloadAsync(CancellationToken ct = default);

        // Снимок: ключ — Id базы
        IReadOnlyDictionary<Guid, RegisteredDatabase> Snapshot { get; }

        // Поиск (с ленивой загрузкой из БД если не найдено в кеше)
        RegisteredDatabase ResolveById(Guid id);
        RegisteredDatabase ResolveByName(string name);
        Task<RegisteredDatabase> ResolveByIdAsync(Guid id, CancellationToken ct = default);
        Task<RegisteredDatabase> ResolveByNameAsync(string name, CancellationToken ct = default);

        bool TryResolveById(Guid id, out RegisteredDatabase db);
        bool TryResolveByName(string name, out RegisteredDatabase db);

        // Регистрация/удаление отдельной базы
        Task RegisterAsync(Guid id, CancellationToken ct = default);
        Task UnregisterAsync(Guid id, CancellationToken ct = default);

        // Тест подключения
        Task<ConnectionTestResult> TestConnectionAsync(string connectionString, DbProviderType provider, CancellationToken ct = default);
    }

    public record ConnectionTestResult(bool Success, string? ServerVersion, string? ErrorMessage, long ResponseTimeMs);
}
