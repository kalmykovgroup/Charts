using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum InaccuracyType
    {
        [Description("Абсолютная")]
        Absolute,
        [Description("Приведенная к ВПИ")]
        Reduced,
        [Description("Относительная")]
        Relative,
    }
}
