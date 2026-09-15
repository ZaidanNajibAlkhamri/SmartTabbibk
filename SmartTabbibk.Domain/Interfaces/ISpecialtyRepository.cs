using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Domain.Interfaces
{
    public interface ISpecialtyRepository : IRepository<Specialty>
    {
        // يتحقق هل يوجد طبيب واحد على الأقل مرتبط بهذا التخصص، قبل السماح بالحذف الفعلي
        Task<bool> IsUsedByAnyDoctorAsync(int specialtyId);
    }
}
