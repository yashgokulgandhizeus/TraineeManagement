using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TrainingDirectory.Api;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Expose a read-only endpoint that returns a profile lookup pattern
app.MapGet("/api/internal/trainees/{id}", (int id) =>
{
    if (id <= 0) return Results.BadRequest(new { Error = "Validation Error: Invalid ID bounds." });
    if (id == 99) return Results.StatusCode(503); // Simulated transient breakdown error for testing retries

    return Results.Ok(new TraineeProfileResponse
    {
        TraineeId = id,
        FullName = $"Trainee Number {id}",
        TierCode = id % 2 == 0 ? "Tier-A" : "Tier-B",
        IsActive = true
    });
});

app.Run("http://localhost:5050");
