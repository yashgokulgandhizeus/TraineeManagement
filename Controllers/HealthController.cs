using Microsoft.AspNetCore.Mvc;

namespace TraineeManagement.Api.Controllers;

[ApiController]
public class HealthController : ControllerBase
{

    [HttpGet("api/health")]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Good",
            Application = "Trainee Management",
            Timestamp = DateTime.Now
        });
    }
}
