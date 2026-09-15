using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTabbibk.Application.DTOs.Medicines;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.API.Controllers
{
    [ApiController]
    [Route("api/medicines")]
    [Authorize]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // UC22 — قائمة الأدوية النشطة (تُستخدم عند إنشاء وصفة) — Doctor
        [HttpGet]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _medicineService.GetActiveAsync();
            return Ok(result);
        }

        // للإدارة فقط — يشمل الأدوية المعطّلة كمان
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _medicineService.GetAllAsync();
            return Ok(result);
        }

        // UC22 — إضافة دواء جديد — Admin فقط
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateMedicineDto dto)
        {
            try
            {
                var result = await _medicineService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetActive), result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // UC22 — تعطيل دواء (بدل الحذف) — Admin فقط
        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _medicineService.DeactivateAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // إعادة تفعيل دواء معطّل — Admin فقط
        [HttpPut("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await _medicineService.ActivateAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
