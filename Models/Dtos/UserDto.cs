namespace cs_apiEcommerce.Models.Dtos;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Username { get; set; } = string.Empty;

    //? Belos props are not good practice to have
    // public string? Password { get; set; }
    // public string? Role { get; set; }
}
