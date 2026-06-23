using TraineeManagement.Worker;
using TraineeManagement.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddHostedService<SubmissionProcessorWorker>();

var host = builder.Build();
host.Run();
