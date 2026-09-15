using SmartTabbibk.Application.DTOs.Auth;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.Auth
{
    /// <summary>
    /// تنفيذ UC0 (تسجيل الدخول) و UC0b (تسجيل حساب)
    /// Business Rules المطبَّقة هنا:
    /// - البريد الإلكتروني فريد (لا يمكن التكرار)
    /// - كلمة المرور مشفّرة دائماً قبل الحفظ
    /// - Patient: يُفعَّل مباشرة | Doctor: IsApproved=false لحد ما Admin يوافق
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterPatientAsync(RegisterPatientDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("البريد الإلكتروني مستخدم بالفعل.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Role = UserRole.Patient,
                Phone = dto.Phone,
                IsActive = true, // Patient يُفعَّل فوراً
                Patient = new Patient
                {
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender
                }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> RegisterDoctorAsync(RegisterDoctorDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("البريد الإلكتروني مستخدم بالفعل.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Role = UserRole.Doctor,
                Phone = dto.Phone,
                IsActive = true,
                Doctor = new Doctor
                {
                    SpecialtyId = dto.SpecialtyId,
                    Bio = dto.Bio,
                    ConsultationFee = dto.ConsultationFee,
                    IsApproved = false // بانتظار اعتماد Admin (Business Rule)
                }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غير صحيحة.");

            if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غير صحيحة.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("الحساب معطّل.");

            return BuildAuthResponse(user);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            return new AuthResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _tokenService.GenerateToken(user),
                ExpiresAt = DateTime.UtcNow.AddHours(2) // مدة صلاحية JWT حسب Business Rules
            };
        }
    }
}
