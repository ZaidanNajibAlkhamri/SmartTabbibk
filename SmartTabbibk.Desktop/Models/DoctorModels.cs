namespace SmartTabbibk.Desktop.Models
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public decimal ConsultationFee { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }

    public class ApproveDoctorRequest
    {
        public bool Approve { get; set; }
    }
}
