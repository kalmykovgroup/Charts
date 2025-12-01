using Npgsql;

namespace Charts.Infrastructure.Helpers;

/// <summary>
/// Maps PostgreSQL SqlState codes to English error messages.
/// Needed because lc_messages option doesn't apply to authentication errors.
/// </summary>
public static class PostgresErrorHelper
{
    private static readonly Dictionary<string, string> SqlStateMessages = new(StringComparer.OrdinalIgnoreCase)
    {
        // Class 28 — Invalid Authorization Specification
        ["28000"] = "Invalid authorization specification",
        ["28P01"] = "Password authentication failed",

        // Class 3D — Invalid Catalog Name
        ["3D000"] = "Database does not exist",

        // Class 3F — Invalid Schema Name
        ["3F000"] = "Schema does not exist",

        // Class 42 — Syntax Error or Access Rule Violation
        ["42501"] = "Permission denied",
        ["42601"] = "Syntax error",
        ["42703"] = "Column does not exist",
        ["42P01"] = "Table does not exist",
        ["42P02"] = "Parameter does not exist",

        // Class 08 — Connection Exception
        ["08000"] = "Connection error",
        ["08003"] = "Connection does not exist",
        ["08006"] = "Connection failure",
        ["08001"] = "Unable to establish connection",
        ["08004"] = "Server rejected connection",

        // Class 53 — Insufficient Resources
        ["53000"] = "Insufficient resources",
        ["53100"] = "Disk full",
        ["53200"] = "Out of memory",
        ["53300"] = "Too many connections",

        // Class 57 — Operator Intervention
        ["57000"] = "Operator intervention",
        ["57014"] = "Query cancelled",
        ["57P01"] = "Server shutdown",
        ["57P02"] = "Crash shutdown",
        ["57P03"] = "Cannot connect now",
    };

    public static string GetMessage(PostgresException ex)
    {
        if (SqlStateMessages.TryGetValue(ex.SqlState ?? "", out var message))
        {
            // Include additional context if available
            if (!string.IsNullOrEmpty(ex.Detail))
                return $"{message}: {ex.Detail}";

            return message;
        }

        // Fallback: return SqlState code with generic message
        return $"Database error (SqlState: {ex.SqlState})";
    }

    public static string GetMessage(Exception ex)
    {
        return ex is PostgresException pgEx ? GetMessage(pgEx) : ex.Message;
    }
}
