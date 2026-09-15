using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTabbibk.Application.DTOs.MedicalRecords;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.API.Controllers
{
    [ApiController]
    [Route("api/medical-records")]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _recordService;

        public MedicalRecordsController(IMedicalRecordService recordService)
        {
            _recordService = recordService;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string CurrentRole =>
            User.FindFirstValue(ClaimTypes.Role)!;

        // UC15 — كتابة تشخيص — Doctor فقط
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> WriteDiagnosis(WriteDiagnosisDto dto)
        {
            try
            {
                var result = await _recordService.WriteDiagnosisAsync(dto, CurrentUserId);
                return CreatedAtAction(nameof(GetByAppointmentId),
                    new { appointmentId = result.AppointmentId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // عرض سجل طبي لموعد معيّن — Patient أو Doctor (صاحب الموعد)
        [HttpGet("{appointmentId}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> GetByAppointmentId(int appointmentId)
        {
            try
            {
                var result = await _recordService.GetByAppointmentIdAsync(appointmentId, CurrentUserId, CurrentRole);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // UC7 — سجلي الطبي الكامل — Patient فقط
        [HttpGet("mine")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _recordService.GetMineAsync(CurrentUserId);
            return Ok(result);
        }

        // UC25 — عرض إداري Read Only — Admin فقط
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetForAdmin(int id)
        {
            try
            {
                var result = await _recordService.GetForAdminAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
