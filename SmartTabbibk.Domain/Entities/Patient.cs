namespace SmartTabbibk.Domain.Entities
{
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? MedicalHistory { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
