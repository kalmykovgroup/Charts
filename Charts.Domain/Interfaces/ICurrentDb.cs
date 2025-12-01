using System.Data.Common;
using Charts.Domain.Contracts.Types;

namespace Charts.Domain.Interfaces
{

    public interface ICurrentDb
    {
        Guid? Key { get; }             // если используешь id 
        DbProviderType Provider { get; }
        ValueTask<DbConnection> OpenConnectionAsync(CancellationToken ct = default);
    }
}
