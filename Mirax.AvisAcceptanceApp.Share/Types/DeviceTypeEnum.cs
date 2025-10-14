using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum DeviceTypeEnum
    {
        [Description("X1")]
        Avis_X1,
        [Description("X4")]
        Avis_X4,
        [Description("X4")]
        Avis_X4M,
        [Description("X4Pro")]
        Avis_X4Pro,
        [Description("X4Pro")]
        Avis_X4ProM,
        [Description("Signal")]
        Signal,
        [Description("Atom")]
        Atom,
        [Description("Axiom")]
        Axiom,
        [Description("None")]
        None,
    }
}
