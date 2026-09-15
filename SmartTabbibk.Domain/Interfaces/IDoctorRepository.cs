using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Domain.Interfaces
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        /// <summary>
        /// بحث متقدم: تخصص + اسم + نطاق سعر + حد أدنى للتقييم
        /// كل المعاملات اختيارية (null = تجاهل الفلتر)
        /// النتيجة تشمل فقط الأطباء المعتمدين (IsApproved = true)
        /// </summary>
        Task<IEnumerable<Doctor>> SearchAsync(
            string? specialty,
            string? name,
            decimal? minPrice,
            decimal? maxPrice,
            double? minRating);

        Task<Doctor?> GetWithDetailsAsync(int doctorId);

        Task<Doctor?> GetByUserIdAsync(int userId);

        Task<IEnumerable<Doctor>> GetPendingApprovalAsync();
    }
}
