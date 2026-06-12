namespace TraineeManagement.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;

[ApiController]
[Route("api/mentor")]
[Authorize]
public class MentorController:ControllerBase
{

    private readonly IMentorService _service;

    public MentorController(IMentorService service)
    {
        _service=service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    [HttpGet("{id}", Name = "GetMentorById")]
    public async Task<IActionResult> GetById(int id)
    {
        if(await _service.GetById(id)==null)
        return NotFound();

       return Ok(await _service.GetById(id));
    
    }

    [HttpPost]
    public async Task<IActionResult> Create(MentorRequest mentorRequest)
    {

        MentorResponse mentorResponse=await _service.Create(mentorRequest);
        return CreatedAtRoute("GetMentorById", new { id = mentorResponse.Id }, mentorResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,MentorRequest mentorRequest)
    {
        MentorResponse mentorResponse=await _service.Update(id,mentorRequest);

        if(mentorResponse==null)
        return NotFound();

        return Ok(mentorResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if(await _service.Delete(id))
        return Ok("deleted");

        else
        return NotFound("for given id there is no record");
    }

}