using System.ComponentModel.DataAnnotations;

namespace SmartTabbibk.Application.DTOs.Reviews
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "يجب تحديد الموعد")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف الموعد غير صحيح")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "التقييم مطلوب")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "التعليق يجب ألا يتجاوز 500 حرف")]
        public string? Comment { get; set; }
    }

    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
