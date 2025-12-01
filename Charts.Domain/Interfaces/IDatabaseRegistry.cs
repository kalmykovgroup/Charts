using Charts.Domain.Contracts;

namespace Charts.Domain.Interfaces
{
    public interface IDatabaseRegistry
    {
        Task InitializeAsync(CancellationToken ct = default);
        Task ReloadAsync(CancellationToken ct = default);

        // Снимок: ключ — Id базы
        IReadOnlyDictionary<Guid, RegisteredDatabase> Snapshot { get; }

        // Поиск
        RegisteredDatabase ResolveById(Guid id);
        RegisteredDatabase ResolveByName(string name);

        bool TryResolveById(Guid id, out RegisteredDatabase db);
        bool TryResolveByName(string name, out RegisteredDatabase db);
    }
}
