namespace TraineeManagement.Api.Messaging;

public class RabbitMqSettings
{
    private readonly IConfiguration _config;

    public RabbitMqSettings(IConfiguration configuration)
    {
        _config=configuration;
    }
    public RabbitMqSettings()
    {
    }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}
