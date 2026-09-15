namespace SmartTabbibk.Application.Interfaces
{
    /// <summary>
    /// مسؤول عن تشفير كلمة المرور والتحقق منها — التنفيذ الفعلي في Infrastructure
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
