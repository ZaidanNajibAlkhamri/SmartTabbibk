using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class SpecialtyRepository : Repository<Specialty>, ISpecialtyRepository
    {
        public SpecialtyRepository(AppDbContext context) : base(context) { }

        public async Task<bool> IsUsedByAnyDoctorAsync(int specialtyId)
        {
            return await _context.Set<Doctor>().AnyAsync(d => d.SpecialtyId == specialtyId);
        }
    }
}
