using Authly.Models.Dtos;

namespace Authly.Services.Dtos
{
    public interface IUserService
    {
        Task<PaginationDto<UserDto>> GetAllAsync(UserQueryDto request);
        Task<UserDto> CreateAsync(CreateUserDto data);

        Task<UserDto?> GetByIdAsync(string id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> UpdateAsync(string id, UpdateUserDto dto);
        Task<bool> DeleteAsync(string id);
        Task<bool> AssignRoleAsync(string id, string role);
    }
}
