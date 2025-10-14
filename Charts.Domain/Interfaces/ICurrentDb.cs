
using Charts.Api.Application.Contracts;
using Charts.Api.Domain.Contracts.Types;
using System.Data.Common;

namespace Charts.Api.Application.Interfaces
{

    public interface ICurrentDb
    {
        Guid? Key { get; }             // если используешь id 
        DbProviderType Provider { get; }
        ValueTask<DbConnection> OpenConnectionAsync(CancellationToken ct = default);
    }
}
