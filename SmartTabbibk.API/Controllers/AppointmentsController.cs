using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTabbibk.Application.DTOs.Appointments;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize] // كل الـ Endpoints هنا تتطلب تسجيل دخول على الأقل
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string CurrentRole =>
            User.FindFirstValue(ClaimTypes.Role)!;

        // UC5 — حجز موعد — Patient فقط
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Book(BookAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.BookAsync(dto, CurrentUserId);
                return CreatedAtAction(nameof(GetById), new { id = result.AppointmentId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // UC6 — إلغاء موعد — Patient فقط
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _appointmentService.CancelAsync(id, CurrentUserId);
                return NoContent();
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

        // UC11 — مواعيدي — Patient أو Doctor
        [HttpGet("mine")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _appointmentService.GetMineAsync(CurrentUserId, CurrentRole);
            return Ok(result);
        }

        // UC12 — تفاصيل موعد — Patient أو Doctor (صاحب الموعد فقط)
        [HttpGet("{id}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _appointmentService.GetByIdAsync(id, CurrentUserId, CurrentRole);
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

        // UC13 — تعديل الأولوية — Doctor فقط
        [HttpPut("{id}/priority")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdatePriority(int id, UpdatePriorityDto dto)
        {
            try
            {
                await _appointmentService.UpdatePriorityAsync(id, dto, CurrentUserId);
                return NoContent();
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

        // UC14 — إدارة حالة الموعد — Doctor فقط
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
        {
            try
            {
                await _appointmentService.UpdateStatusAsync(id, dto, CurrentUserId);
                return NoContent();
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
    }
}
