using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/task-assignments")]
[Authorize]
public class TaskAssignmentController : ControllerBase
{
    private readonly ITaskAssignmentService _service;

    public TaskAssignmentController(ITaskAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        TaskAssignmentResponse taskAssignmentResponse = await _service.GetById(id);

        if (taskAssignmentResponse == null)
        {
            return NotFound("Task Assignment with given id is not available");
        }

        return Ok(taskAssignmentResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskAssignmentRequest request)
    {
        TaskAssignmentResponse taskAssignmentResponse = await _service.TaskAssignment(request);

        if (taskAssignmentResponse == null)
        {
            return BadRequest("either given id's are not available or due date is already expired");
        }

        return Ok(taskAssignmentResponse);
    }

    [HttpPut("{id}/{status}")]
    public async Task<IActionResult> Update(int id, AssignmentStatus status)
    {
        TaskAssignmentResponse taskAssignmentResponse = await _service.UpdateStatus(id, status);

        if (taskAssignmentResponse == null)
        {
            return BadRequest("either id is not found or status is invalid");
        }

        return Ok(taskAssignmentResponse);
    }
}
