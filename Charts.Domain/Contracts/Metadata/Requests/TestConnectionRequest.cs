namespace Charts.Domain.Contracts.Metadata.Requests;

public class TestConnectionRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Provider { get; set; } = "PostgreSql";
}
