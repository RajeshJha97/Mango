using Mango.Services.AuthAPI.Models.DTO;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDTO registrationRequestDto);
        Task<LoginResponseDTO>Login(LoginRequestDTO loginRequestDto);
        Task<bool> AssignRole(string email, string role);
    }
}
