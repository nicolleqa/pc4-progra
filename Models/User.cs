using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    public string? UserId { get; set; }

    public string? Name { get; set; }
}