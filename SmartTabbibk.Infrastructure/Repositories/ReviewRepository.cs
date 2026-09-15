using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet
                .Include(r => r.Patient).ThenInclude(p => p.User)
                .Where(r => r.DoctorId == doctorId)
                .OrderByDescending(r => r.ReviewId)
                .ToListAsync();
        }

        public async Task<bool> ExistsForAppointmentAsync(int patientId, int appointmentId)
        {
            return await _dbSet.AnyAsync(r => r.PatientId == patientId && r.AppointmentId == appointmentId);
        }
    }
}
