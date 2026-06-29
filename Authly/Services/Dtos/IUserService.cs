using Authly.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace Authly.Services.Dtos
{
    public interface IUserService
    {
        Task<PaginationDto<UserDto>> GetAllAsync(UserQueryDto request);
        Task<UserDto> CreateAsync(CreateUserDto data, string actorId);

        Task<UserDto?> GetByIdAsync(string id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> UpdateAsync(string id, UpdateUserDto dto, string actorId);
        Task<UserDto> UpdateImageAsync(string id, IFormFile file, string actorId);
        Task<bool> DeleteAsync(string id, bool hardDelete = false);
        Task<bool> AssignRoleAsync(string id, string role);
        Task<bool> ChangePasswordAsync(string id, ChangePasswordDto dto);
        Task<bool> ResetPasswordAsync(string id, ResetPasswordDto dto);
    }
}
