using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum ClimateTestStatus
    {
        [Description("Запущен")]
        Running,
        [Description("Остановлен")]
        Stoped,
        [Description("Готов к запуску")]
        ReadyToRun,
        [Description("Завершён с ошибкой")]
        Aborted,
        [Description("В процессе")]
        Worked,
        [Description("Завершён")]
        Compelted
    }
}
