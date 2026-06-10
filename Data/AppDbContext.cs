using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
      
    }

    public DbSet<Trainee> Trainees => Set<Trainee>();

    public DbSet<User> Users=> Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id=1,
            UserName="Admin",
            Email="admin@gmail.com",
            PasswordHash=BCrypt.Net.BCrypt.HashPassword("admin@123"),
            Role="Admin"
    
        });

    }
}