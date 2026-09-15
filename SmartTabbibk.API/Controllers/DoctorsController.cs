using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTabbibk.Application.DTOs.Doctors;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.API.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // UC3 — بحث متقدم (تخصص، اسم، نطاق سعر، حد أدنى تقييم)
        // مفتوح لأي مستخدم مسجّل دخول (Patient أساساً)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Search(
            [FromQuery] string? specialty,
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] double? minRating)
        {
            var result = await _doctorService.SearchAsync(specialty, name, minPrice, maxPrice, minRating);
            return Ok(result);
        }

        // UC4 — عرض ملف طبيب كامل
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
            {
                var result = await _doctorService.GetProfileAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // قائمة الأطباء بانتظار الاعتماد — Admin فقط
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _doctorService.GetPendingApprovalAsync();
            return Ok(result);
        }

        // UC20 — اعتماد/رفض طبيب — Admin فقط
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, ApproveDoctorDto dto)
        {
            try
            {
                await _doctorService.ApproveDoctorAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // UC21 — تعديل بيانات إدارية للطبيب — Admin فقط
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
        {
            try
            {
                await _doctorService.UpdateDoctorAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
