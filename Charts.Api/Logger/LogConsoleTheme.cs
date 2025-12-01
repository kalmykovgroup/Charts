using Serilog.Sinks.SystemConsole.Themes;

namespace Charts.Api.Logger
{
    public class LogConsoleTheme
    {
        public static AnsiConsoleTheme DarkTheme => new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[37m",          // Белый
            [ConsoleThemeStyle.SecondaryText] = "\x1b[90m", // Тёмно-серый
            [ConsoleThemeStyle.TertiaryText] = "\x1b[90m",
            [ConsoleThemeStyle.Invalid] = "\x1b[91m",       // Красный
            [ConsoleThemeStyle.Null] = "\x1b[35m",          // Фиолетовый
            [ConsoleThemeStyle.Name] = "\x1b[36m",          // Голубой
            [ConsoleThemeStyle.String] = "\x1b[32m",        // Зелёный
            [ConsoleThemeStyle.Number] = "\x1b[33m",        // Жёлтый
            [ConsoleThemeStyle.Boolean] = "\x1b[33m",
            [ConsoleThemeStyle.Scalar] = "\x1b[37m",
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m",
            [ConsoleThemeStyle.LevelDebug] = "\x1b[37m",
            [ConsoleThemeStyle.LevelInformation] = "\x1b[32m",
            [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",
            [ConsoleThemeStyle.LevelError] = "\x1b[31m",
            [ConsoleThemeStyle.LevelFatal] = "\x1b[31;1m",
        });
    }
}
