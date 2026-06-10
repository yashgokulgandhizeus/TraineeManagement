using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.Models;


public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string UserName { get; set; }

    public string Email { get; set; }
    
    public string PasswordHash { get; set; }

    public string Role { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public User()
    {
        CreatedDate=DateTime.Now;
        UpdatedDate=DateTime.Now;
    }
}