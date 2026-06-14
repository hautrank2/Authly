using Authly.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Authly.Models.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public string? AvtUrl { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime? LatestAccess { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static UserDto FromUser(User user) => new()
        {
            Id = user.Id!,
            Name = user.Name,
            Birthday = user.Birthday,
            AvtUrl = user.AvtUrl,
            Username = user.Username,
            Role = user.Role,
            LatestAccess = user.LatestAccess,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
    }

    public class UserQueryDto
    {
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;

        public string? Name { get; set; }
        public int? StartAge { get; set; }
        public int? EndAge { get; set; }
        public UserRole? Role { get; set; }
    }

    public record CreateUserDto(
        [Required] string Name,
        [Required] string Username,
        [Required, MinLength(6)] string Password,
        string Birthday, // YYYY-MM-DD
        IFormFile? Avatar,
        UserRole Role = UserRole.Dev
    );

    public record UpdateUserDto(
        string? Name,
        string? Birthday,
        string? AvtUrl,
        UserRole? Role
    );
}
