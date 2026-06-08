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
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        TraineeResponse t=await _service.GetById(id);
        
        if(t==null)
        {
            return NotFound("Not found");
        }

        return Ok(t);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] String search)
    {
        List<TraineeResponse> t=await _service.Search(search);
        
        if(t==null)
        {
            return NotFound("Not found");
        }

        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create(CreateTraineeRequest trainee)
    {
     
        return Ok(_service.CreateTrainee(trainee));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,UpdateTraineeRequest trainee)
    {
        TraineeResponse traineeResponse=await _service.Update(id,trainee);
        if(traineeResponse==null)
        {
            return NotFound();
        }

        else return Ok(traineeResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool res=await _service.Delete(id);

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