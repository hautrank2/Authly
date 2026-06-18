using Authly.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace Authly.Services.Dtos
{
    public interface IUserService
    {
        Task<PaginationDto<UserDto>> GetAllAsync(UserQueryDto request);
        Task<UserDto> CreateAsync(CreateUserDto data);

        Task<UserDto?> GetByIdAsync(string id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> UpdateAsync(string id, UpdateUserDto dto);
        Task<UserDto> UpdateImageAsync(string id, IFormFile file);
        Task<bool> DeleteAsync(string id);
        Task<bool> AssignRoleAsync(string id, string role);
    }
}
