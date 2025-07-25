using System.ComponentModel.DataAnnotations;

namespace cs_apiEcommerce.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? Role { get; set; }
    
}
