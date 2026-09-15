using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class MedicineRepository : Repository<Medicine>, IMedicineRepository
    {
        public MedicineRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Medicine>> GetActiveAsync()
        {
            return await _dbSet.Where(m => m.IsActive).OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<bool> IsUsedInPrescriptionAsync(int medicineId)
        {
            return await _context.Set<Prescription>().AnyAsync(p => p.MedicineId == medicineId);
        }

        public async Task<bool> ExistsAsync(string name, string strength, string form)
        {
            return await _dbSet.AnyAsync(m =>
                m.Name == name && m.Strength == strength && m.Form == form);
        }
    }
}
