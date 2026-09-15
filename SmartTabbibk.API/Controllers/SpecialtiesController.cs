using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTabbibk.Application.DTOs.Specialties;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.API.Controllers
{
    [ApiController]
    [Route("api/specialties")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        // عام (بدون توكن) — يُستخدم في فورم تسجيل الطبيب قبل ما يكون عنده حساب أصلاً
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _specialtyService.GetAllAsync();
            return Ok(result);
        }

        // إضافة تخصص جديد — Admin فقط
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateSpecialtyDto dto)
        {
            var result = await _specialtyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), result);
        }

        // حذف حقيقي (Hard Delete) — مسموح فقط لو غير مستخدم من أي طبيب — Admin فقط
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _specialtyService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
