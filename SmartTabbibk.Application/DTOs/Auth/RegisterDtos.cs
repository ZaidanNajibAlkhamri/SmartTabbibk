using System.ComponentModel.DataAnnotations;

namespace SmartTabbibk.Application.DTOs.Auth
{
    public class RegisterPatientDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "الاسم يجب أن يكون بين 2 و100 حرف")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }

    public class RegisterDoctorDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "التخصص مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار تخصص صحيح")]
        public int SpecialtyId { get; set; }

        [StringLength(500, ErrorMessage = "النبذة التعريفية يجب ألا تتجاوز 500 حرف")]
        public string? Bio { get; set; }

        [Range(0, 100000, ErrorMessage = "سعر الكشف يجب أن يكون قيمة موجبة معقولة")]
        public decimal ConsultationFee { get; set; }
    }
}
