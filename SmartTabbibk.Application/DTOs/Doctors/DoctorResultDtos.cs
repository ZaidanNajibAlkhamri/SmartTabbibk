namespace SmartTabbibk.Application.DTOs.Doctors
{
    public class DoctorSearchResultDto
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public decimal ConsultationFee { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }

    public class DoctorProfileDto
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public decimal ConsultationFee { get; set; }
        public double AverageRating { get; set; }
        public List<ReviewSummaryDto> RecentReviews { get; set; } = new();
    }

    public class ReviewSummaryDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
