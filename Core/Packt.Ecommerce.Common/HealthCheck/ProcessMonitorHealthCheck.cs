using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Packt.Ecommerce.Common.HealthCheck
{
    public class ProcessMonitorHealthCheck : IHealthCheck
    {
        private readonly string processName;

        public ProcessMonitorHealthCheck(string processName) => this.processName = processName;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            Process[] pname = Process.GetProcessesByName(this.processName);
            if (pname.Length == 0)
            {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, description: $"Process with the name {this.processName} is not running."));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }
        }
    }
}
