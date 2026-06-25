using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Dtos;

namespace TraineeManagement.Api.Models;

[Index(nameof(Status), Name = "IX_LearningTasks_Status")]
[Index(nameof(DueDate), Name = "IX_LearningTasks_DueDate")]
public class LearningTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 

    public string Title { get; set; }

    public string Description { get; set; }

    public string ExpectedTechStack { get; set; }

    public DateTime DueDate { get; set; }

    public Dtos.TaskStatus Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public LearningTask()
    {
        CreatedDate = DateTime.Now;
        UpdatedDate = DateTime.Now;
    }
}
