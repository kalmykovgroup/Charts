using System.ComponentModel;

namespace Mirax.AvisAcceptanceApp.Share.Types
{
    public enum ClimateEventType
    {
        [Description("Выход на точку")]
        EntranceToThePoint,
        [Description("Поддержание температуры")]
        TemperatureMaintenance,
        [Description("Запись в базу данных")]
        SetDatatoDb,
        [Description("Отключение")]
        ClimateStop,
        [Description("Нет")]
        None,
    }
}
