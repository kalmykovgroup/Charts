namespace Charts.Domain.Contracts
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Exception? Exception { get; init; }

        public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
        public static ApiResponse<T> Fail(string msg, Exception? ex = null) => new() { Success = false, ErrorMessage = msg, Exception = ex };
    }
}
