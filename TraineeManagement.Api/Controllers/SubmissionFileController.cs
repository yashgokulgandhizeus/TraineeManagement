using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Services;


namespace TraineeManagement.Api.Controllers
{
    [Authorize(Roles ="Trainee,Mentor,Admin")]
    [ApiController]
    [Route("api")]
    public class SubmissionFileController : ControllerBase
    {
        private readonly ISubmissionFileService _service;
        public SubmissionFileController(ISubmissionFileService service)
        {
            _service = service;
        }


        [HttpPost("submissions/{submissionId}/files")]
        public async Task<IActionResult> Upload(int submissionId, IFormFile file)
        {
            var response = await _service.UploadAsync(submissionId, file);

            if (response == null)
            {
                return NotFound("Submission not found");
            }

            return Accepted(new
            {
                Message = "File uploaded successfully. Processing context queued.",
                CorrelationId = response.CorrelationId,
                FileId = response.Id
            });
        }

        [HttpGet("submission-files/{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var result = await _service.DownloadAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return File(result.Value.stream, result.Value.contentType, result.Value.fileName);
        }

        [HttpDelete("submission-files/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
