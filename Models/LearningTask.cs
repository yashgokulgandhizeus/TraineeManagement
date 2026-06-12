using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.Models;

public class LearningTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

    public string Title {get; set;}

    public string Description { get; set; }

    public string ExpectedTechStack { get; set; }
    
    public DateTime DueDate { get; set; }

    public string Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public LearningTask()
    {
        CreatedDate=DateTime.Now;
        UpdatedDate=DateTime.Now;
    }
}