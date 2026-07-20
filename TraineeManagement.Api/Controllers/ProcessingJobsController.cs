using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[Authorize(Roles ="Trainee,Mentor,Admin")]
[ApiController]
[Route("api/processing-jobs")]
public class ProcessingJobsController : ControllerBase
{
    private readonly IProcessingJobsService _service;

    public ProcessingJobsController(IProcessingJobsService service)
    {
        _service=service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobStatus(int id)
    {
        // Reports authoritative database state rather than broker state
        var job = await _service.GetJobStatus(id);

        if (job == null)
        {
            return NotFound(new { Message = $"No processing job tracked matching Id: {id}" });
        }

        return Ok(
        job);
    }
}
