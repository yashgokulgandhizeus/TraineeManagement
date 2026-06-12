using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.Models;

public class Mentor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

    public string FirstName {get; set;}

    public string LastName { get; set; }

    public string Email { get; set; }
    
    public string Experties { get; set; }

    public string Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public Mentor()
    {
        CreatedDate=DateTime.Now;
        UpdatedDate=DateTime.Now;
    }
}