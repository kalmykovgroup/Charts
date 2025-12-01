using System.Data.Common;

namespace Charts.Domain.Interfaces
{
    public interface IDbProviderRegistry
    {
        DbProviderFactory GetFactory(string provider);
    }
}
