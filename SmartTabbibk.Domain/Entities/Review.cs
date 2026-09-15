namespace SmartTabbibk.Domain.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }

        // UNIQUE(PatientId, AppointmentId) يُطبَّق في Infrastructure (EF Core Fluent API)
    }
}
