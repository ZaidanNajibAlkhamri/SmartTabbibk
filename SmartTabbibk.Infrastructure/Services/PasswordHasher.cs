using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.Infrastructure.Services
{
    /// <summary>
    /// يستخدم BCrypt.Net-Next — لازم تضيف الـ NuGet Package:
    /// dotnet add SmartTabbibk.Infrastructure package BCrypt.Net-Next
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
