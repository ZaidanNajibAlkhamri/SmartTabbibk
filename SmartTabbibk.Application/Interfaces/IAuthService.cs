using SmartTabbibk.Application.DTOs.Auth;

namespace SmartTabbibk.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterPatientAsync(RegisterPatientDto dto);
        Task<AuthResponseDto> RegisterDoctorAsync(RegisterDoctorDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
