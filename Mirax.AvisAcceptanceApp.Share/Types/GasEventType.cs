using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum GasEventType
    {
        [Description("Начата подача")]
        SupplyStart,
        [Description("Остановка подачи")]
        SupplyStop,
        [Description("Нет")]
        None,
    }
}
