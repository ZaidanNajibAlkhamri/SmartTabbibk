using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Doctor>> SearchAsync(
            string? specialty, string? name, decimal? minPrice, decimal? maxPrice, double? minRating)
        {
            // فلاتر قابلة للترجمة لـ SQL مباشرة (تخصص، اسم، سعر) تتنفذ في قاعدة البيانات
            var query = _dbSet
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Reviews)
                .Where(d => d.IsApproved) // Business Rule: أطباء معتمدون فقط
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(specialty))
                query = query.Where(d => d.Specialty.Name.Contains(specialty));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.User.Name.Contains(name));

            if (minPrice.HasValue)
                query = query.Where(d => d.ConsultationFee >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(d => d.ConsultationFee <= maxPrice.Value);

            var doctors = await query.ToListAsync();

            // فلتر التقييم (Average) يتنفذ في الذاكرة بعد الجلب،
            // لأن EF Core لا يترجم Average على مجموعة قد تكون فارغة بسهولة عبر SQL
            if (minRating.HasValue)
            {
                doctors = doctors
                    .Where(d => d.Reviews.Any() && d.Reviews.Average(r => r.Rating) >= minRating.Value)
                    .ToList();
            }

            return doctors;
        }

        public async Task<Doctor?> GetWithDetailsAsync(int doctorId)
        {
            return await _dbSet
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Reviews)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<Doctor?> GetByUserIdAsync(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<IEnumerable<Doctor>> GetPendingApprovalAsync()
        {
            return await _dbSet
                .Include(d => d.User)
                .Include(d => d.Specialty)
                // Business Rule: مرفوض (User.IsActive=false) يختلف عن بانتظار المراجعة
                // فقط الحسابات النشطة وغير المعتمدة بعد تظهر هنا
                .Where(d => !d.IsApproved && d.User.IsActive)
                .ToListAsync();
        }
    }
}
