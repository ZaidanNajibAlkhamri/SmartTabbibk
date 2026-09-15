namespace SmartTabbibk.Domain.Entities
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; } = null!;

        public string? Bio { get; set; }
        public decimal ConsultationFee { get; set; }

        // ملاحظة: IsApproved هو المعيار الوحيد لأهلية الحجز في V1 (لا يوجد IsActive هنا)
        public bool IsApproved { get; set; } = false;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
