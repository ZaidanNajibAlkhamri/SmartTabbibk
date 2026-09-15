using SmartTabbibk.Application.DTOs.Medicines;

namespace SmartTabbibk.Application.Interfaces
{
    public interface IMedicineService
    {
        Task<IEnumerable<MedicineResponseDto>> GetActiveAsync();

        // للإدارة فقط — يشمل الأدوية المعطّلة كمان
        Task<IEnumerable<MedicineResponseDto>> GetAllAsync();

        Task<MedicineResponseDto> CreateAsync(CreateMedicineDto dto);

        Task DeactivateAsync(int medicineId);

        Task ActivateAsync(int medicineId);
    }
}
