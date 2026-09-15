using SmartTabbibk.Application.DTOs.Reviews;

namespace SmartTabbibk.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto, int patientUserId);

        Task<IEnumerable<ReviewResponseDto>> GetByDoctorIdAsync(int doctorId);

        // Hard Delete حقيقي — Admin فقط، لحذف تقييم مسيء أو غير لائق
        Task DeleteAsync(int reviewId);
    }
}
