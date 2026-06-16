using Authly.Models.Dtos;

namespace Authly.Services.Dtos
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
