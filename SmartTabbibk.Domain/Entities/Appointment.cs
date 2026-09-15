using SmartTabbibk.Domain.Enums;

namespace SmartTabbibk.Domain.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        // V1: يحدَّد يدوياً من المريض ويعدَّل من الطبيب
        // مستقبلاً: عبر IPriorityEvaluator بدون تغيير هذا الحقل
        public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

        public string? Symptoms { get; set; }

        public MedicalRecord? MedicalRecord { get; set; }
        public Review? Review { get; set; }
    }
}
