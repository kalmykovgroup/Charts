using Charts.Domain.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Charts.Infrastructure.Startup
{
    public sealed class ReadinessHealthCheck : IHealthCheck
    {
        private readonly IAppReadiness _ready;
        public ReadinessHealthCheck(IAppReadiness ready) => _ready = ready;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext _, CancellationToken __)
            => Task.FromResult(_ready.Ready ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("Not ready"));
    }
}
