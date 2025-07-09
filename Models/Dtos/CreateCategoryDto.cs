using System.ComponentModel.DataAnnotations;

namespace cs_apiEcommerce.Models.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "The name is required.")]
    [MaxLength(50, ErrorMessage = " The name cannot be more than 50 characters.")]
    [MinLength(3,ErrorMessage = " The name cannot have les than 3 characters.")]
    public string Name { get; set; } = string.Empty;
}
