namespace Charts.Api.Domain.Contracts.Types
{
    public enum DatabaseAvailability
    {
        Unknown = 0, // ещё не проверяли
        Online = 1, // подключение открылось
        Offline = 2  // подключение не удалось
                     // при желании можно расширить: AuthFailed, NetworkError, Timeout и т.п.
    }
}
