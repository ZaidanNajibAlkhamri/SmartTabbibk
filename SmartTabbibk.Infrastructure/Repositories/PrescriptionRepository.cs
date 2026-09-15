using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Prescription>> GetByRecordIdAsync(int recordId)
        {
            return await _dbSet
                .Include(p => p.Medicine)
                .Where(p => p.RecordId == recordId)
                .ToListAsync();
        }
    }
}
