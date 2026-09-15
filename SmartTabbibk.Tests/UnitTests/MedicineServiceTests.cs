using SmartTabbibk.Application.DTOs.Medicines;
using SmartTabbibk.Application.Features.Medicines;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Tests.Fakes;
using Xunit;

namespace SmartTabbibk.Tests.UnitTests
{
    /// <summary>
    /// يغطي Business Rule: UNIQUE(Name, Strength, Form) — يجب رفض الدواء المكرر
    /// قبل الوصول لقاعدة البيانات أصلاً (فحص صريح في Application، مش استثناء EF Core)
    /// </summary>
    public class MedicineServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDuplicateCombinationExists()
        {
            var repo = new FakeMedicineRepository();
            repo.Medicines.Add(new Medicine { MedicineId = 1, Name = "Paracetamol", Strength = "500mg", Form = "Tablet", IsActive = true });
            var service = new MedicineService(repo);

            var dto = new CreateMedicineDto { Name = "Paracetamol", Strength = "500mg", Form = "Tablet" };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
            Assert.Contains("مسجّل بالفعل", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldSucceed_WhenCombinationIsUnique()
        {
            var repo = new FakeMedicineRepository();
            repo.Medicines.Add(new Medicine { MedicineId = 1, Name = "Paracetamol", Strength = "500mg", Form = "Tablet", IsActive = true });
            var service = new MedicineService(repo);

            // نفس الاسم، لكن تركيز مختلف — تركيبة فريدة، المفروض تنجح
            var dto = new CreateMedicineDto { Name = "Paracetamol", Strength = "1000mg", Form = "Tablet" };

            var result = await service.CreateAsync(dto);

            Assert.Equal("Paracetamol", result.Name);
            Assert.Equal(2, repo.Medicines.Count);
        }
    }
}
