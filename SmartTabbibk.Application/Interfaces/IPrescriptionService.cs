using SmartTabbibk.Application.DTOs.Prescriptions;

namespace SmartTabbibk.Application.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResponseDto> CreateAsync(CreatePrescriptionDto dto, int doctorUserId);

        Task<IEnumerable<PrescriptionItemResultDto>> GetByRecordIdAsync(int recordId);
    }
}
