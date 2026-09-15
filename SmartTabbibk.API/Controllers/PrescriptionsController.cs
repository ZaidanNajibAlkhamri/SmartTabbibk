using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTabbibk.Application.DTOs.Prescriptions;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.API.Controllers
{
    [ApiController]
    [Route("api/prescriptions")]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // UC16 — إنشاء وصفة طبية — Doctor فقط
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create(CreatePrescriptionDto dto)
        {
            try
            {
                var result = await _prescriptionService.CreateAsync(dto, CurrentUserId);

                // لو فيه تحذيرات (أدوية اتجاهلت) نرجّع 207، وإلا 201 عادي
                if (result.Warnings.Any())
                    return StatusCode(207, result);

                return CreatedAtAction(nameof(GetByRecordId), new { recordId = result.RecordId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // عرض وصفات سجل طبي معيّن — Patient أو Doctor
        [HttpGet("{recordId}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> GetByRecordId(int recordId)
        {
            var result = await _prescriptionService.GetByRecordIdAsync(recordId);
            return Ok(result);
        }
    }
}
