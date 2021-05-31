using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Packt.Ecommerce.Common.HealthCheck
{
    public static class ProcessMonitorHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddProcessMonitorHealthCheck(
            this IHealthChecksBuilder builder,
            string processName = default,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(
               name ?? "ProcessMonitor",
               sp => new ProcessMonitorHealthCheck(processName),
               failureStatus,
               tags));
        }
    }
}
