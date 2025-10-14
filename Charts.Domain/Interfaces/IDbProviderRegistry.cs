using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charts.Api.Domain.Interfaces
{
    public interface IDbProviderRegistry
    {
        DbProviderFactory GetFactory(string provider);
    }
}
