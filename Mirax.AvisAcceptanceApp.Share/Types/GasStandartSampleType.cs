using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum GasStandartSampleType
    {
        [Description("Не выбрано")]
        None,
        [Description("ПНГ")]
        NullGas,
        [Description("ПГС")]
        SpanGas,
    }
}
