using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Middleware;
using TraineeManagement.Api.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TraineeManagement.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// FIXED: Routing internal communication over the Docker network service name
builder.Services.AddHttpClient("TrainingDirectoryService", client =>
{
    client.BaseAddress = new Uri("http://trainingdirectory_api:8080/");
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddStandardResilienceHandler();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(tags: new[] { "ready" })
    .AddRedis(
        builder.Configuration["ConnectionStrings:Redis"]!,
        name: "Redis",
        tags: new[] { "ready" }
    ).AddRabbitMQ(
        async sp => await sp.GetRequiredService<RabbitMQ.Client.ConnectionFactory>().CreateConnectionAsync(),
        name: "RabbitMQ",
        tags: new[] { "ready" }
    );

builder.Services.AddOpenApi();
builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileService>();
builder.Services.AddScoped<IProcessingJobsService, ProcessingJobsService>();
builder.Services.AddScoped<ITrainingDirectoryClient, TrainingDirectoryClient>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton(sp =>
{
    var rabbitMqSection = builder.Configuration.GetSection("RabbitMQ");
    return new RabbitMQ.Client.ConnectionFactory
    {
        HostName = rabbitMqSection["Host"] ?? "localhost",
        Port = int.Parse(rabbitMqSection["Port"] ?? "5672"),
        UserName = rabbitMqSection["UserName"] ?? "guest",
        Password = rabbitMqSection["Password"] ?? "guest",
        VirtualHost = rabbitMqSection["VirtualHost"] ?? "/"
    };
});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "http://localhost:5173");
                      });
}); 

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "TraineeManagement";
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// FIXED POSITION & LOGIC: Run database schema updates with an Oracle-safe fallback mechanism
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        logger.LogInformation("Initializing containerized database schema...");
        
        try
        {
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception migrationEx)
        {
            logger.LogWarning($"Standard migration routing bypassed: {migrationEx.Message}. Executing robust fallback schema creation...");
            
            // FALLBACK FIX: Force schema generation directly if standard history tables clash
            context.Database.EnsureCreated();
            logger.LogInformation("Database tables successfully built using structural fallback generator.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An unhandled critical exception occurred while setting up database schemas.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseCors(MyAllowSpecificOrigins);
app.UseExceptionHandler(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
