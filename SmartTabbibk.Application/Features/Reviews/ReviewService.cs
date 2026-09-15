using SmartTabbibk.Application.DTOs.Reviews;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.Reviews
{
    /// <summary>
    /// تنفيذ UC8 — تقييم الطبيب
    /// Business Rules: بعد Completed فقط، لا تقييم لموعد ملغي، تقييم واحد لكل موعد
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository)
        {
            _reviewRepository = reviewRepository;
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
        }

        public async Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto, int patientUserId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(patientUserId)
                ?? throw new InvalidOperationException("حساب المريض غير موجود.");

            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId)
                ?? throw new KeyNotFoundException("الموعد غير موجود.");

            if (appointment.PatientId != patient.PatientId)
                throw new UnauthorizedAccessException("هذا الموعد لا يخص هذا الحساب.");

            if (appointment.Status != AppointmentStatus.Completed)
                throw new InvalidOperationException("لا يمكن التقييم إلا بعد اكتمال الموعد.");

            if (await _reviewRepository.ExistsForAppointmentAsync(patient.PatientId, dto.AppointmentId))
                throw new InvalidOperationException("تم تقييم هذا الموعد بالفعل.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("التقييم يجب أن يكون بين 1 و5.");

            var review = new Review
            {
                DoctorId = appointment.DoctorId,
                PatientId = patient.PatientId,
                AppointmentId = dto.AppointmentId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                PatientName = patient.User?.Name ?? string.Empty,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetByDoctorIdAsync(int doctorId)
        {
            var reviews = await _reviewRepository.GetByDoctorIdAsync(doctorId);

            return reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,
                PatientName = r.Patient?.User?.Name ?? string.Empty,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();
        }

        public async Task DeleteAsync(int reviewId)
        {
            // Hard Delete حقيقي هنا: التقييم رأي شخصي وليس سجلاً طبياً أو مالياً،
            // فحذفه (مثلاً لمخالفته سياسة المحتوى) لا يفقد النظام أي بيانات جوهرية
            var review = await _reviewRepository.GetByIdAsync(reviewId)
                ?? throw new KeyNotFoundException("التقييم غير موجود.");

            _reviewRepository.Delete(review);
            await _reviewRepository.SaveChangesAsync();
        }
    }
}
