using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TraineesController : ControllerBase
{
    private readonly ITraineeService _service;


    public TraineesController(ITraineeService traineeService)
    {
        _service = traineeService;
    }


    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PaginationQueryRequest request)
    {
        return Ok(await _service.GetAll(request));

    }

    [HttpGet("{id}", Name = "GetTraineeById")]
    public async Task<IActionResult> GetById(int id)
    {
        TraineeResponse traineeResponse = await _service.GetById(id);

        if (traineeResponse == null)
        {
            return Ok();
        }

        return Ok(traineeResponse);
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateTraineeRequest trainee)
    {

        TraineeResponse traineeResponse = await _service.CreateTrainee(trainee);

        return CreatedAtRoute("GetTraineeById", new { id = traineeResponse.Id }, traineeResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTraineeRequest trainee)
    {
        TraineeResponse traineeResponse = await _service.Update(id, trainee);
        if (traineeResponse == null)
        {
            return NotFound();
        }

        else return Ok(traineeResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool response = await _service.Delete(id);

        if (response)
        {
            return Ok();
        }

        else
        {
            return NotFound();
        }
    }


}
