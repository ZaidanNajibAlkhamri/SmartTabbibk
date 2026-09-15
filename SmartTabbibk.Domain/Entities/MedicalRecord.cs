namespace SmartTabbibk.Domain.Entities
{
    public class MedicalRecord
    {
        public int RecordId { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public string Diagnosis { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // لا وصفة إلزامية — قد يكون فارغاً
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
