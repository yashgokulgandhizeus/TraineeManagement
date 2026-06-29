using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReadyStatus()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var response = new
        {
            Status = report.Status == HealthStatus.Healthy ? "Good" : "Degraded",
            Application = "Trainee Management",
            Type = "Readiness Check",
            Timestamp = DateTime.Now,
            TotalDurationMs = report.TotalDuration.TotalMilliseconds,
            
            Dependencies = report.Entries.Select(entry => new
            {
                Component = entry.Key,
                Status = entry.Value.Status.ToString(),
                DurationMs = entry.Value.Duration.TotalMilliseconds
            })
        };

        if (report.Status == HealthStatus.Unhealthy)
        {
            return StatusCode(503, response);
        }

        return Ok(response);
    }

    [HttpGet("live")]
    public IActionResult GetLiveStatus()
    {
        var response = new
        {
            Status = "Good",
            Application = "Trainee Management",
            Type = "Liveness Check",
            Timestamp = DateTime.Now
        };

        return Ok(response);
    }
}
