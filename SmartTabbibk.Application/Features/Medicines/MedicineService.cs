using SmartTabbibk.Application.DTOs.Medicines;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.Medicines
{
    /// <summary>
    /// تنفيذ UC22 — إدارة الأدوية
    /// </summary>
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicineService(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<IEnumerable<MedicineResponseDto>> GetActiveAsync()
        {
            var medicines = await _medicineRepository.GetActiveAsync();
            return medicines.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<MedicineResponseDto>> GetAllAsync()
        {
            // للإدارة فقط — يشمل النشط وغير النشط (يعتمد على IRepository<T>.GetAllAsync الأساسية)
            var medicines = await _medicineRepository.GetAllAsync();
            return medicines.Select(MapToDto).ToList();
        }

        public async Task<MedicineResponseDto> CreateAsync(CreateMedicineDto dto)
        {
            // Business Rule: UNIQUE(Name, Strength, Form) — نتحقق قبل الحفظ
            // بدل الاعتماد على استثناء EF Core (يحافظ على استقلالية Application عن تفاصيل قاعدة البيانات)
            if (await _medicineRepository.ExistsAsync(dto.Name, dto.Strength, dto.Form))
                throw new InvalidOperationException("هذا الدواء بنفس التركيز والشكل الصيدلاني مسجّل بالفعل.");

            var medicine = new Medicine
            {
                Name = dto.Name,
                Strength = dto.Strength,
                Form = dto.Form,
                IsActive = true
            };

            await _medicineRepository.AddAsync(medicine);
            await _medicineRepository.SaveChangesAsync();

            return MapToDto(medicine);
        }

        public async Task DeactivateAsync(int medicineId)
        {
            var medicine = await _medicineRepository.GetByIdAsync(medicineId)
                ?? throw new KeyNotFoundException("الدواء غير موجود.");

            // Business Rule: تعطيل بدل الحذف — الدواء يبقى محفوظاً في الوصفات القديمة
            medicine.IsActive = false;
            _medicineRepository.Update(medicine);
            await _medicineRepository.SaveChangesAsync();
        }

        public async Task ActivateAsync(int medicineId)
        {
            var medicine = await _medicineRepository.GetByIdAsync(medicineId)
                ?? throw new KeyNotFoundException("الدواء غير موجود.");

            medicine.IsActive = true;
            _medicineRepository.Update(medicine);
            await _medicineRepository.SaveChangesAsync();
        }

        private static MedicineResponseDto MapToDto(Medicine m) => new()
        {
            MedicineId = m.MedicineId,
            Name = m.Name,
            Strength = m.Strength,
            Form = m.Form,
            IsActive = m.IsActive
        };
    }
}
