using System.ComponentModel.DataAnnotations;

namespace SmartTabbibk.Application.DTOs.MedicalRecords
{
    public class WriteDiagnosisDto
    {
        [Required(ErrorMessage = "يجب تحديد الموعد")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف الموعد غير صحيح")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "التشخيص مطلوب")]
        [StringLength(1000, MinimumLength = 2)]
        public string Diagnosis { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class MedicalRecordResponseDto
    {
        public int RecordId { get; set; }
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PrescriptionSummaryDto> Prescriptions { get; set; } = new();
    }

    public class PrescriptionSummaryDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Instructions { get; set; }
    }
}
