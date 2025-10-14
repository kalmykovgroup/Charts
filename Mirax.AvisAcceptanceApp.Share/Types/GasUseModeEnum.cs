using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum GasUseModeEnum
    {
        [Description("Нет")]
        None,
        [Description("Ручной")]
        Manual,
        [Description("Автоматический")]
        StandAlone

    }
}
