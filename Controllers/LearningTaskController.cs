namespace TraineeManagement.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Dtos;

[ApiController]
[Route("api/learning-task")]
public class LearningTaskController : ControllerBase
{

    private readonly ILearningTaskService _service;

    public LearningTaskController(ILearningTaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    [HttpGet("{id}",Name ="GetLearningTaskById")]
    public async Task<IActionResult> GetById(int id)
    {
        if (await _service.GetById(id) == null)
            return NotFound();

        return Ok(await _service.GetById(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create(LearningTaskRequest learningTaskRequest)
    {
        LearningTaskResponse Response = await _service.Create(learningTaskRequest);

        return CreatedAtRoute("GetLearningTaskById", new { id = Response.Id }, Response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LearningTaskRequest learningTaskRequest)
    {
        LearningTaskResponse Response = await _service.Update(id, learningTaskRequest);

        if (Response == null)
            return NotFound();

        return Ok(Response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _service.Delete(id))
            return Ok("deleted");

        else
            return NotFound("for given id there is no record");
    }

}