using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Domain.Interfaces
{
    public interface IMedicineRepository : IRepository<Medicine>
    {
        Task<IEnumerable<Medicine>> GetActiveAsync();
        Task<bool> IsUsedInPrescriptionAsync(int medicineId);

        Task<bool> ExistsAsync(string name, string strength, string form);
    }
}
