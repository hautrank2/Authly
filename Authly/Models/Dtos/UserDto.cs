using Authly.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Authly.Models.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Birthday { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string? AvtUrl { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime? LatestAccess { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserQueryDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? Name { get; set; }
        public int? StartAge { get; set; }
        public int? EndAge { get; set; }
        public UserRole? Role { get; set; }
    }

    public record CreateUserDto(
        [Required] string Name,
        [Required] string Username,
        [Required, MinLength(6)] string Password,
        string? Birthday,
        string? AvtUrl,
        UserRole Role = UserRole.Dev
    );

    public record UpdateUserDto(
        string? Name,
        string? Birthday,
        string? AvtUrl,
        UserRole? Role
    );
}
