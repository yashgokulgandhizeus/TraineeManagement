using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration Configuration) : base(options)
    {
        _configuration = Configuration;
    }
    
    public AppDbContext()
{
}
    public DbSet<Trainee> Trainees => Set<Trainee>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Mentor> Mentors => Set<Mentor>();
    public DbSet<LearningTask> LearningTasks => Set<LearningTask>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<ProcessingJob> ProcessingJobs => Set<ProcessingJob>();

    public DbSet<SubmissionFile> SubmissionFiles {get ; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LearningTask>()
            .Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        modelBuilder.Entity<Mentor>()
            .Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        modelBuilder.Entity<Review>()
            .Property(r => r.ReviewStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Submission>()
            .Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<TaskAssignment>()
            .Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Trainee>()
            .Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<SubmissionFile>().HasOne(f=> f.Submission).WithMany(s=>s.Files).HasForeignKey(f=> f.SubmissionId);

        // Fetch seed configurations cleanly
        var adminUser = _configuration["SeedData:AdminUsername"] ?? "Admin";
        var adminEmail = _configuration["SeedData:AdminEmail"] ?? "admin@gmail.com";
        var adminPassword = _configuration["SeedData:AdminPassword"] ?? "admin@123";

        // AUTOMATED HASH GENERATION:
        // Uses a fixed work-factor salt so EF Core snapshot model values remain stable across migration evaluations
        string fixedSalt = BCrypt.Net.BCrypt.GenerateSalt(11);
        string secureAdminHash = BCrypt.Net.BCrypt.HashPassword(adminPassword, fixedSalt);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            UserName = adminUser,
            Email = adminEmail,
            PasswordHash = secureAdminHash, // Assigned programmatically
            Role = UserRole.Admin
        });
    }
}
