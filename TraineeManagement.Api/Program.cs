using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Middleware;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Messaging;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient("TrainingDirectoryService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5050/");
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddStandardResilienceHandler();


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<ITraineeService,TraineeService>();

builder.Services.AddScoped<IAuthService,AuthService>();

builder.Services.AddScoped<IMentorService,MentorService>();

builder.Services.AddScoped<ILearningTaskService,LearningTaskService>();

builder.Services.AddScoped<ITaskAssignmentService,TaskAssignmentService>();

builder.Services.AddScoped<ISubmissionService,SubmissionService>();

builder.Services.AddScoped<IReviewService,ReviewService>();

builder.Services.AddScoped<IFileStorageService , LocalFileStorageService>();
builder.Services.AddScoped<ISubmissionFileService , SubmissionFileService>();

builder.Services.AddScoped<IProcessingJobsService , ProcessingJobsService>();

builder.Services.AddScoped<ITrainingDirectoryClient , TrainingDirectoryClient>();

builder.Services.AddScoped<ICacheService,CacheService>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));


var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "http://localhost:5173");
                      });
}); 


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Converts all enums to string globally across your Web API JSON payloads
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });



// Extract the connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext with MySQL support
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

// Retrieve the connection string
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// Register Redis Distributed Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "TraineeManagement";
});


// Configure JWT Authentication
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

// Configure the HTTP request pipeline.
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
