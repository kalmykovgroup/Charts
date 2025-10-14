namespace Api.Shared.Extensions
{
    public static class StringColorExtensions
    {
        private const string Reset = "\u001b[0m"; // Сброс цвета

        // Существующие стандартные цвета
        public static string Black(this string text) => "\u001b[30m" + text + Reset;
        public static string Red(this string text) => "\u001b[31m" + text + Reset;
        public static string Green(this string text) => "\u001b[32m" + text + Reset;
        public static string Yellow(this string text) => "\u001b[33m" + text + Reset;
        public static string Blue(this string text) => "\u001b[34m" + text + Reset;
        public static string Magenta(this string text) => "\u001b[35m" + text + Reset;
        public static string Cyan(this string text) => "\u001b[36m" + text + Reset;
        public static string White(this string text) => "\u001b[37m" + text + Reset;

        // Существующие яркие цвета
        public static string BrightBlack(this string text) => "\u001b[90m" + text + Reset;
        public static string BrightRed(this string text) => "\u001b[91m" + text + Reset;
        public static string BrightGreen(this string text) => "\u001b[92m" + text + Reset;
        public static string BrightYellow(this string text) => "\u001b[93m" + text + Reset;
        public static string BrightBlue(this string text) => "\u001b[94m" + text + Reset;
        public static string BrightMagenta(this string text) => "\u001b[95m" + text + Reset;
        public static string BrightCyan(this string text) => "\u001b[96m" + text + Reset;
        public static string BrightWhite(this string text) => "\u001b[97m" + text + Reset;

        // Существующие стили
        public static string Bold(this string text) => "\u001b[1m" + text + Reset;
        public static string Underline(this string text) => "\u001b[4m" + text + Reset;

        // Новые стили
        public static string Italic(this string text) => "\u001b[3m" + text + Reset;
        public static string Blink(this string text) => "\u001b[5m" + text + Reset; // Может не поддерживаться в некоторых терминалах
        public static string Invert(this string text) => "\u001b[7m" + text + Reset; // Инверсия цвета текста и фона

        // Новые цвета (256-цветная палитра)
        public static string Orange(this string text) => "\u001b[38;5;208m" + text + Reset; // Оранжевый
        public static string Purple(this string text) => "\u001b[38;5;129m" + text + Reset; // Пурпурный
        public static string Teal(this string text) => "\u001b[38;5;37m" + text + Reset; // Бирюзовый
        public static string Olive(this string text) => "\u001b[38;5;100m" + text + Reset; // Оливковый

        // RGB-цвета
        public static string CustomRGB(this string text, int r, int g, int b) => $"\u001b[38;2;{r};{g};{b}m" + text + Reset;
        public static string Coral(this string text) => "\u001b[38;2;255;127;127m" + text + Reset; // Коралловый
        public static string SkyBlue(this string text) => "\u001b[38;2;135;206;235m" + text + Reset; // Небесно-голубой

        // Фоновые цвета
        public static string BgBlack(this string text) => "\u001b[40m" + text + Reset;
        public static string BgRed(this string text) => "\u001b[41m" + text + Reset;
        public static string BgGreen(this string text) => "\u001b[42m" + text + Reset;
        public static string BgYellow(this string text) => "\u001b[43m" + text + Reset;
        public static string BgBlue(this string text) => "\u001b[44m" + text + Reset;
        public static string BgMagenta(this string text) => "\u001b[45m" + text + Reset;
        public static string BgCyan(this string text) => "\u001b[46m" + text + Reset;
        public static string BgWhite(this string text) => "\u001b[47m" + text + Reset;

        // Яркие фоновые цвета
        public static string BgBrightBlack(this string text) => "\u001b[100m" + text + Reset;
        public static string BgBrightRed(this string text) => "\u001b[101m" + text + Reset;
        public static string BgBrightGreen(this string text) => "\u001b[102m" + text + Reset;
        public static string BgBrightYellow(this string text) => "\u001b[103m" + text + Reset;
        public static string BgBrightBlue(this string text) => "\u001b[104m" + text + Reset;
        public static string BgBrightMagenta(this string text) => "\u001b[105m" + text + Reset;
        public static string BgBrightCyan(this string text) => "\u001b[106m" + text + Reset;
        public static string BgBrightWhite(this string text) => "\u001b[107m" + text + Reset;

        // Комбинированные стили (цвет + стиль)
        public static string BoldRed(this string text) => "\u001b[1;31m" + text + Reset;
        public static string UnderlineGreen(this string text) => "\u001b[4;32m" + text + Reset;
        public static string ItalicBlue(this string text) => "\u001b[3;34m" + text + Reset;
    }
}
