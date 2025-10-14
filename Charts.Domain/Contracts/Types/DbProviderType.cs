using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charts.Api.Domain.Contracts.Types
{
    public enum DbProviderType
    {

        Unknown,
        PostgreSql = 1,
        SqlServer = 2,
        MySql = 3,
        Sqlite = 4,
        Oracle = 5,
    } 
}
