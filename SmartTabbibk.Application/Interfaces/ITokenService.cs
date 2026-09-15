using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Application.Interfaces
{
    /// <summary>
    /// مسؤول فقط عن إصدار وفك تشفير JWT — التنفيذ الفعلي في Infrastructure
    /// </summary>
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
