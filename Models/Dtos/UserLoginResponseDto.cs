namespace cs_apiEcommerce.Models.Dtos;

public class UserLoginResponseDto
{
    public UserDataDto? User { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}
