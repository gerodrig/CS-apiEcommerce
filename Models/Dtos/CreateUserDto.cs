using System.ComponentModel.DataAnnotations;

namespace cs_apiEcommerce.Models.Dtos;

public class CreateUserDto
{
 [Required(ErrorMessage = "The username field is required.")]
  public string? Username { get; set; }

  [Required(ErrorMessage = "the name field is required.")]
  public string? Name { get; set; }

  [Required(ErrorMessage = "The password field is required.")]
  public string? Password { get; set; }

  [Required(ErrorMessage = "The role fiels is required.")]
  public string? Role { get; set; }
}
