using System.Text.Json.Serialization;

namespace Charts.Api.Application.Contracts.Charts.Dtos
{
    public sealed record RawPointDto(long Time, double? Value);
}
