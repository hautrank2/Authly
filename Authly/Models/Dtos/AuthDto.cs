namespace Authly.Models.Dtos
{
    public record LoginRequestDto(string Username, string Password);

    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
