using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraineesController : ControllerBase
{
    
    public static List<Trainee> trainees=new List<Trainee>();
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(trainees);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Trainee t=trainees.FirstOrDefault(x=>x.Id==id);
        
        if(trainees==null)
        {
            return Ok("Not found");
        }

        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create(Trainee trainee)
    {
        trainee.Id=trainees.Count+1;
        trainee.CreatedDate=DateTime.Now;
        trainee.UpdatedDate=DateTime.Now;

        trainees.Add(trainee);

        return Ok(trainee);
    }
}