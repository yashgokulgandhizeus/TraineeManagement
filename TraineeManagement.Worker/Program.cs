using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TraineeManagement.Api.Data;
using TraineeManagement.Worker;
using TraineeManagement.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

// 1. Register Configuration so AppDbContext can read seed data keys
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddHostedService<SubmissionProcessorWorker>();

// 🟢 FIX 2: Register Redis Distributed Cache inside the Worker to resolve internal dependency leaks
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "TraineeManagement";
});

// 2. Register DbContext with the matching Oracle MySQL provider
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

var host = builder.Build();
host.Run();
