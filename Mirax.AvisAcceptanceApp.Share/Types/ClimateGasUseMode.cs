using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum ClimateGasUseMode
    {
        [Description("Нет")]
        None,
        [Description("Ручной")]
        Manual,
        [Description("Автоматический")]
        StandAlone
    }
}
