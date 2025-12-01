using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Interfaces;
using MediatR;

namespace Charts.Application.Handlers.Metadata.Databases
{
    public class TestConnectionHandler(
        IDatabaseRegistry registry
    ) : IRequestHandler<TestConnectionCommand, ApiResponse<ConnectionTestResult>>
    {
        public async Task<ApiResponse<ConnectionTestResult>> Handle(TestConnectionCommand command, CancellationToken ct)
        {
            if (!Enum.TryParse<DbProviderType>(command.Provider, true, out var providerType))
            {
                return ApiResponse<ConnectionTestResult>.Ok(new ConnectionTestResult(
                    Success: false,
                    ServerVersion: null,
                    ErrorMessage: $"Unknown provider: {command.Provider}",
                    ResponseTimeMs: 0
                ));
            }

            var result = await registry.TestConnectionAsync(command.ConnectionString, providerType, ct);
            return ApiResponse<ConnectionTestResult>.Ok(result);
        }
    }
}
