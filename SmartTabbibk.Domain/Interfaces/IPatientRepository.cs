using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByUserIdAsync(int userId);
    }
}
