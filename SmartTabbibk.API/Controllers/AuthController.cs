using Microsoft.AspNetCore.Mvc;
using SmartTabbibk.Application.DTOs.Auth;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // UC0b — تسجيل مريض (Patient: تفعيل مباشر)
        [HttpPost("register/patient")]
        public async Task<IActionResult> RegisterPatient(RegisterPatientDto dto)
        {
            try
            {
                var result = await _authService.RegisterPatientAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // UC0b — تسجيل طبيب (Doctor: بانتظار اعتماد Admin)
        [HttpPost("register/doctor")]
        public async Task<IActionResult> RegisterDoctor(RegisterDoctorDto dto)
        {
            try
            {
                var result = await _authService.RegisterDoctorAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // UC0 — تسجيل الدخول
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
