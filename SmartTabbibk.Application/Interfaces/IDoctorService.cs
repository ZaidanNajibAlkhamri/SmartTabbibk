using SmartTabbibk.Application.DTOs.Doctors;

namespace SmartTabbibk.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorSearchResultDto>> SearchAsync(
            string? specialty, string? name, decimal? minPrice, decimal? maxPrice, double? minRating);

        Task<DoctorProfileDto> GetProfileAsync(int doctorId);

        Task<IEnumerable<DoctorSearchResultDto>> GetPendingApprovalAsync();

        Task ApproveDoctorAsync(int doctorId, ApproveDoctorDto dto);

        Task UpdateDoctorAsync(int doctorId, UpdateDoctorDto dto);
    }
}
