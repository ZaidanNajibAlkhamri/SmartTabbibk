using System.ComponentModel.DataAnnotations;

namespace SmartTabbibk.Application.DTOs.Appointments
{
    public class BookAppointmentDto
    {
        [Required(ErrorMessage = "يجب تحديد الطبيب")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف الطبيب غير صحيح")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "تاريخ ووقت الموعد مطلوب")]
        public DateTime AppointmentDate { get; set; }

        [StringLength(1000, ErrorMessage = "وصف الأعراض يجب ألا يتجاوز 1000 حرف")]
        public string? Symptoms { get; set; }

        // اختياري: لو المريض ما حددش، يُستخدم IPriorityEvaluator كـ Fallback
        public string? Priority { get; set; }
    }

    public class UpdatePriorityDto
    {
        [Required(ErrorMessage = "قيمة الأولوية مطلوبة")]
        [RegularExpression("^(Normal|Urgent)$", ErrorMessage = "الأولوية يجب أن تكون Normal أو Urgent")]
        public string Priority { get; set; } = string.Empty;
    }

    public class UpdateStatusDto
    {
        [Required(ErrorMessage = "قيمة الحالة مطلوبة")]
        [RegularExpression("^(Pending|Confirmed|Completed|Cancelled)$", ErrorMessage = "قيمة الحالة غير صحيحة")]
        public string Status { get; set; } = string.Empty;
    }

    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
    }
}
