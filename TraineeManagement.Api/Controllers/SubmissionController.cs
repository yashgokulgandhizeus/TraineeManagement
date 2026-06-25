using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/submissions")]
[Authorize]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionService _service;

    public SubmissionController(ISubmissionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        SubmissionResponse submissionResponse = await _service.GetById(id);

        if (submissionResponse == null)
            return NotFound("Submission with given id is not available");

        return Ok(submissionResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SubmissionRequest request)
    {
        SubmissionResponse submissionResponse = await _service.Create(request);

        if (submissionResponse == null)
            return BadRequest("given task assignment id is not available.");

        return Ok(submissionResponse);
    }
}
