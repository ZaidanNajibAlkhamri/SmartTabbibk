using System.ComponentModel.DataAnnotations;

namespace SmartTabbibk.Application.DTOs.Specialties
{
    public class SpecialtyDto
    {
        public int SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateSpecialtyDto
    {
        [Required(ErrorMessage = "اسم التخصص مطلوب")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Keywords { get; set; }
    }
}
