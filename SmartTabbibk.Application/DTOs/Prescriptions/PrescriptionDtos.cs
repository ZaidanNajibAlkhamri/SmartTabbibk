using System.ComponentModel.DataAnnotations;

namespace SmartTabbibk.Application.DTOs.Prescriptions
{
    public class PrescriptionItemDto
    {
        [Required(ErrorMessage = "يجب اختيار الدواء")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف الدواء غير صحيح")]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "الجرعة مطلوبة")]
        [StringLength(200)]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "المدة مطلوبة")]
        [StringLength(100)]
        public string Duration { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Instructions { get; set; }
    }

    public class CreatePrescriptionDto
    {
        [Required(ErrorMessage = "يجب تحديد السجل الطبي")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف السجل الطبي غير صحيح")]
        public int RecordId { get; set; }

        [Required(ErrorMessage = "يجب إضافة دواء واحد على الأقل")]
        [MinLength(1, ErrorMessage = "يجب إضافة دواء واحد على الأقل")]
        public List<PrescriptionItemDto> Items { get; set; } = new();
    }

    public class PrescriptionResponseDto
    {
        public int RecordId { get; set; }
        public List<PrescriptionItemResultDto> SavedItems { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class PrescriptionItemResultDto
    {
        public int PrescriptionId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Instructions { get; set; }
    }
}
