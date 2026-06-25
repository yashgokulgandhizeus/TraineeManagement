using TraineeManagement.Api.Messaging;

namespace TraineeManagement.Api.Services;

public interface IMessagePublisher
{
    Task PublishSubmissionProcessingAsync(SubmissionProcessingRequested message);
}