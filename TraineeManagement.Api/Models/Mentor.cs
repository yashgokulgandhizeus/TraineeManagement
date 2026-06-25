using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Dtos;

namespace TraineeManagement.Api.Models;

[Index(nameof(Email), IsUnique = true, Name = "IX_Mentors_Email")]
[Index(nameof(Status), Name = "IX_Mentors_Status")]
public class Mentor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Experties { get; set; }

    public MentorStatus Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public Mentor()
    {
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }
}
