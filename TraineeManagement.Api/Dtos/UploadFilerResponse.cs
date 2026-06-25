namespace TraineeManagement.Api.Dtos;

public class UploadFileResponse{
    public int Id { get; set; }

    public string? OriginalFileName { get; set; }

    public string? StoredFileName { get; set; }

    public string? ContentType { get; set; }
    public string? Checksum { get; set; }

    public long? FileSize { get; set; }

    public Guid CorrelationId { get; set; } 
}