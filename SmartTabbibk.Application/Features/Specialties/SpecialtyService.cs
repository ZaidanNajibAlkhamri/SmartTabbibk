using SmartTabbibk.Application.DTOs.Specialties;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.Specialties
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _specialtyRepository;

        public SpecialtyService(ISpecialtyRepository specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        public async Task<IEnumerable<SpecialtyDto>> GetAllAsync()
        {
            var specialties = await _specialtyRepository.GetAllAsync();
            // .ToList() ضرورية هنا: XmlSerializer مايقدرش يتعامل مع نوع Select()
            // الداخلي (Deferred Iterator)، ومحتاج نوع ملموس زي List<T> له Constructor عام
            return specialties.Select(s => new SpecialtyDto { SpecialtyId = s.SpecialtyId, Name = s.Name }).ToList();
        }

        public async Task<SpecialtyDto> CreateAsync(CreateSpecialtyDto dto)
        {
            var specialty = new Specialty { Name = dto.Name, Keywords = dto.Keywords };
            await _specialtyRepository.AddAsync(specialty);
            await _specialtyRepository.SaveChangesAsync();

            return new SpecialtyDto { SpecialtyId = specialty.SpecialtyId, Name = specialty.Name };
        }

        public async Task DeleteAsync(int specialtyId)
        {
            var specialty = await _specialtyRepository.GetByIdAsync(specialtyId)
                ?? throw new KeyNotFoundException("التخصص غير موجود.");

            // Hard Delete حقيقي هنا (بعكس باقي الموارد) لأن التخصص كيان تصنيفي بحت
            // لا يحمل بيانات طبية حساسة، فحذفه آمن طالما مفيش طبيب مرتبط بيه فعلياً
            if (await _specialtyRepository.IsUsedByAnyDoctorAsync(specialtyId))
                throw new InvalidOperationException("لا يمكن حذف التخصص لوجود أطباء مرتبطين به. عدّل تخصصهم أولاً.");

            _specialtyRepository.Delete(specialty);
            await _specialtyRepository.SaveChangesAsync();
        }
    }
}
