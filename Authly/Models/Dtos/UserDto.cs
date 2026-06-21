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
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

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
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
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
        public bool IsDeleted { get; set; } = false;
    }

    public record CreateUserDto(
        [Required, MinLength(1), MaxLength(100)] string Name,
        [Required, MinLength(3), MaxLength(30),
         RegularExpression(@"^[a-zA-Z0-9_-]+$",
            ErrorMessage = "Username chỉ được chứa chữ cái, số, dấu gạch dưới và gạch ngang.")]
        string Username,
        [Required,
         RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm ít nhất 1 số và 1 ký tự đặc biệt.")]
        string Password,
        string Birthday,   // YYYY-MM-DD
        IFormFile? Avatar,
        UserRole Role = UserRole.Dev
    );

    public record UpdateUserDto(
        [MinLength(1), MaxLength(100)] string? Name,
        string? Birthday,
        UserRole? Role
    );

    public record ChangePasswordDto(
        [Required] string CurrentPassword,
        [Required,
         RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm ít nhất 1 số và 1 ký tự đặc biệt.")]
        string NewPassword
    );

    public record ResetPasswordDto(
        [Required,
         RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm ít nhất 1 số và 1 ký tự đặc biệt.")]
        string NewPassword
    );
}
