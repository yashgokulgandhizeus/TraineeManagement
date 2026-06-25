namespace TraineeManagement.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Services;

[ApiController]
[Route("api/review")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewController(IReviewService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reviews = await _service.GetAll();
        return Ok(reviews);
    }

    [HttpGet("{id}", Name = "GetReviewById")]
    public async Task<IActionResult> GetById(int id)
    {
        var reviewResponse = await _service.GetById(id);

        if (reviewResponse == null)
            return NotFound();

        return Ok(reviewResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReviewRequest reviewRequest)
    {
        ReviewResponse reviewResponse = await _service.Create(reviewRequest);

        if (reviewResponse == null)
            return BadRequest("Invalid SubmissionId or matching system assignment criteria missing.");

        return CreatedAtRoute("GetReviewById", new { id = reviewResponse.Id }, reviewResponse);
    }
}
