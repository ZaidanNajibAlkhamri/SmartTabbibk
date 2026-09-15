using System.ComponentModel.DataAnnotations;

namespace SmartTabbibk.Application.DTOs.Medicines
{
    public class CreateMedicineDto
    {
        [Required(ErrorMessage = "اسم الدواء مطلوب")]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "التركيز مطلوب")]
        [StringLength(50)]
        public string Strength { get; set; } = string.Empty;

        [Required(ErrorMessage = "الشكل الصيدلاني مطلوب")]
        [StringLength(50)]
        public string Form { get; set; } = string.Empty;
    }

    public class MedicineResponseDto
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
