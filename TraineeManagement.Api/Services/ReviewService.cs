using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Exceptions;

namespace TraineeManagement.Api.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AppDbContext context, ILogger<ReviewService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReviewResponse> Create(ReviewRequest request)
    {
        Submission submission = await _context.Submissions
            .Include(e => e.TaskAssignment)
            .ThenInclude(ta => ta.Mentor)
            .FirstOrDefaultAsync(e => e.Id == request.SubmissionId);

        if (submission == null || submission.TaskAssignment == null || submission.TaskAssignment.Mentor == null)
        {
            _logger.LogCritical("Failed to create review. Submission, TaskAssignment, or Mentor not found for SubmissionId: " + request.SubmissionId);

            throw new NotFoundException($"Submission, assignment matrix, or assigned mentor records not found for SubmissionId: {request.SubmissionId}");
        }

        Review review = new Review
        {
            SubmissionId = request.SubmissionId,
            MentorId = submission.TaskAssignment.MentorId,
            ReviewStatus = request.ReviewStatus,
            Score = request.Score,
            Feedback = request.Feedback
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Review created successfully with Status: " + review.ReviewStatus + ", assigned Id: " + review.Id + " by MentorId: " + review.MentorId);

        return new ReviewResponse
        {
            Id = review.Id,
            SubmissionId = review.SubmissionId,
            MentorId = review.MentorId,
            MentorName = $"{submission.TaskAssignment.Mentor.FirstName} {submission.TaskAssignment.Mentor.LastName}",
            Feedback = review.Feedback,
            Score = review.Score,
            ReviewStatus = review.ReviewStatus,
            ReviewedDate = review.ReviewedDate
        };
    }

    public async Task<List<ReviewResponse>> GetAll()
    {
        _logger.LogInformation("Fetching all reviews from the database.");

        return await _context.Reviews
            .Include(e => e.Mentor)
            .Select(e => new ReviewResponse
            {
                Id = e.Id,
                SubmissionId = e.SubmissionId,
                MentorId = e.MentorId,
                MentorName = $"{e.Mentor.FirstName} {e.Mentor.LastName}",
                Feedback = e.Feedback,
                Score = e.Score,
                ReviewStatus = e.ReviewStatus,
                ReviewedDate = e.ReviewedDate
            })
            .ToListAsync();
    }

    public async Task<ReviewResponse> GetById(int id)
    {
        Review review = await _context.Reviews
            .Include(e => e.Mentor)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (review == null)
        {
            _logger.LogCritical("Review not found with Id: " + id);

            throw new NotFoundException($"Review evaluation with Id {id} was not found.");
        }

        _logger.LogInformation("Review found and retrieved for Id: " + id);

        return new ReviewResponse
        {
            Id = review.Id,
            SubmissionId = review.SubmissionId,
            MentorId = review.MentorId,
            MentorName = $"{review.Mentor.FirstName} {review.Mentor.LastName}",
            Feedback = review.Feedback,
            Score = review.Score,
            ReviewStatus = review.ReviewStatus,
            ReviewedDate = review.ReviewedDate
        };
    }
}
