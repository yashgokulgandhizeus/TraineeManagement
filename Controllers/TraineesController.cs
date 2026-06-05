using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Services;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraineesController : ControllerBase
{
    private readonly ITraineeService _service;

    public TraineesController(ITraineeService traineeService)
    {
        _service=traineeService;
    }

    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        TraineeResponse t=_service.GetById(id);
        
        if(t==null)
        {
            return Ok("Not found");
        }

        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create(CreateTraineeRequest trainee)
    {
     
        return Ok(_service.CreateTrainee(trainee));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id,UpdateTraineeRequest trainee)
    {
        TraineeResponse traineeResponse=_service.Update(id,trainee);
        if(traineeResponse==null)
        {
            return NotFound();
        }

        else return Ok(traineeResponse);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        bool res=_service.Delete(id);

        if(res)
        {
            return Ok();
        }

        else
        {
            return NotFound();
        }
    } 
}