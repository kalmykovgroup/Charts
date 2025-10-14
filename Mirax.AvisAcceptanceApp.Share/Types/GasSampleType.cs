using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum GasSampleType
    {
        [Description("Без учета")]
        None,
        [Description("ПНГ")]
        NullGas,
        [Description("ПГС")]
        CalibrationGas
    }
}
