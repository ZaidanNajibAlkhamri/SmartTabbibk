using SmartTabbibk.Application.DTOs.Doctors;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.Doctors
{
    /// <summary>
    /// تنفيذ UC3 (بحث)، UC4 (عرض ملف)، UC20 (اعتماد)، UC21 (إدارة)
    /// </summary>
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<IEnumerable<DoctorSearchResultDto>> SearchAsync(
            string? specialty, string? name, decimal? minPrice, decimal? maxPrice, double? minRating)
        {
            var doctors = await _doctorRepository.SearchAsync(specialty, name, minPrice, maxPrice, minRating);

            return doctors.Select(d => new DoctorSearchResultDto
            {
                DoctorId = d.DoctorId,
                Name = d.User.Name,
                SpecialtyName = d.Specialty.Name,
                ConsultationFee = d.ConsultationFee,
                AverageRating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0,
                ReviewsCount = d.Reviews.Count
            }).ToList();
        }

        public async Task<DoctorProfileDto> GetProfileAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetWithDetailsAsync(doctorId)
                ?? throw new KeyNotFoundException("الطبيب غير موجود.");

            return new DoctorProfileDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.User.Name,
                SpecialtyName = doctor.Specialty.Name,
                Bio = doctor.Bio,
                ConsultationFee = doctor.ConsultationFee,
                AverageRating = doctor.Reviews.Any() ? doctor.Reviews.Average(r => r.Rating) : 0,
                RecentReviews = doctor.Reviews
                    .OrderByDescending(r => r.ReviewId)
                    .Take(5)
                    .Select(r => new ReviewSummaryDto { Rating = r.Rating, Comment = r.Comment })
                    .ToList()
            };
        }

        public async Task<IEnumerable<DoctorSearchResultDto>> GetPendingApprovalAsync()
        {
            var doctors = await _doctorRepository.GetPendingApprovalAsync();

            return doctors.Select(d => new DoctorSearchResultDto
            {
                DoctorId = d.DoctorId,
                Name = d.User.Name,
                SpecialtyName = d.Specialty.Name,
                ConsultationFee = d.ConsultationFee,
                AverageRating = 0,
                ReviewsCount = 0
            }).ToList();
        }

        public async Task ApproveDoctorAsync(int doctorId, ApproveDoctorDto dto)
        {
            // نحتاج بيانات User عشان نقدر نعطّل الحساب في حالة الرفض
            var doctor = await _doctorRepository.GetWithDetailsAsync(doctorId)
                ?? throw new KeyNotFoundException("الطبيب غير موجود.");

            if (dto.Approve)
            {
                // اعتماد: الطبيب يظهر في البحث ويستقبل حجوزات
                doctor.IsApproved = true;
            }
            else
            {
                // رفض: IsApproved تبقى false (كانت أصلاً كذلك)، لكن الفرق الحقيقي
                // إننا نعطّل حساب المستخدم (User.IsActive = false) عشان:
                // 1) يختفي فعلياً من قائمة "بانتظار المراجعة" (بدل ما يفضل عالق فيها للأبد)
                // 2) يمنعه من تسجيل الدخول مرة أخرى بنفس الحساب المرفوض
                doctor.IsApproved = false;
                doctor.User.IsActive = false;
            }

            _doctorRepository.Update(doctor);
            await _doctorRepository.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(int doctorId, UpdateDoctorDto dto)
        {
            var doctor = await _doctorRepository.GetWithDetailsAsync(doctorId)
                ?? throw new KeyNotFoundException("الطبيب غير موجود.");

            // UC21 المعتمد: Admin يقدر يعدّل التخصص والبيانات الإدارية مباشرة
            if (dto.SpecialtyId.HasValue) doctor.SpecialtyId = dto.SpecialtyId.Value;
            if (dto.Bio != null) doctor.Bio = dto.Bio;
            if (dto.ConsultationFee.HasValue) doctor.ConsultationFee = dto.ConsultationFee.Value;
            // IsActive حقل في Users وليس Doctors — يُطبَّق على doctor.User إذا لُحّق بالكيان
            if (dto.IsActive.HasValue) doctor.User.IsActive = dto.IsActive.Value;

            _doctorRepository.Update(doctor);
            await _doctorRepository.SaveChangesAsync();
        }
    }
}
