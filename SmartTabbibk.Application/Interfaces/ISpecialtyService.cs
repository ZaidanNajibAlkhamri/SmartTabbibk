using SmartTabbibk.Application.DTOs.Specialties;

namespace SmartTabbibk.Application.Interfaces
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<SpecialtyDto>> GetAllAsync();

        Task<SpecialtyDto> CreateAsync(CreateSpecialtyDto dto);

        // Hard Delete حقيقي — مسموح فقط لو التخصص غير مستخدم من أي طبيب
        Task DeleteAsync(int specialtyId);
    }
}
