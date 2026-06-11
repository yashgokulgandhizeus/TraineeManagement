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

    public TraineesController(ITraineeService TraineeService)
    {
        _service = TraineeService;
    }


    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]PaginationQueryRequest Request)
    {
        return Ok(await _service.GetAll(Request));

    }

    [HttpGet("{id}",Name ="GetTraineeById")]
    public async Task<IActionResult> GetById(int Id)
    {
        TraineeResponse t = await _service.GetById(Id);

        if (t == null)
        {
            return Ok();
        }

        return Ok(t);
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateTraineeRequest Trainee)
    {

        TraineeResponse TraineeResponse=await _service.CreateTrainee(Trainee);

        return CreatedAtRoute("GetTraineeById", new { id = TraineeResponse.Id }, TraineeResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTraineeRequest Trainee)
    {
        TraineeResponse traineeResponse = await _service.Update(id, Trainee);
        if (traineeResponse == null)
        {
            return NotFound();
        }

        else return Ok(traineeResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int Id)
    {
        bool res = await _service.Delete(Id);

        if (res)
        {
            return Ok();
        }

        else
        {
            return NotFound();
        }
    }


}