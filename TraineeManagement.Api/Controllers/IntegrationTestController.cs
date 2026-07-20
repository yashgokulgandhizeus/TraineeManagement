using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/integration")]
[Authorize(Roles ="Trainee,Mentor,Admin")]
public class IntegrationTestController : ControllerBase
{
    private readonly ITrainingDirectoryClient _directoryClient;

    public IntegrationTestController(ITrainingDirectoryClient directoryClient)
    {
        _directoryClient = directoryClient;
    }

    [HttpGet("lookup/{traineeId}")]
    public async Task<IActionResult> TestLookup(int traineeId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _directoryClient.GetProfileAsync(traineeId, cancellationToken);
            return Ok(profile);
        }
        catch (ArgumentException valEx)
        {
            // TASK 3.19: Do not retry or execute fallback loops for strict schema/validation failures
            return BadRequest(new { Message = valEx.Message });
        }
        catch (Exception)
        {
            // 🟢 TASK 3.19: Fallback Behavior Definition when the internal service is completely down
            return Ok(new DirectoryProfileDto
            {
                TraineeId = traineeId,
                FullName = "Offline Cache Fallback Profile",
                TierCode = "Tier-Unknown",
                IsActive = false,
                Track = "Maintenance Mode Active"
            });
        }
    }
}
