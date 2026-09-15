namespace SmartTabbibk.Application.DTOs.Doctors
{
    public class ApproveDoctorDto
    {
        // true = اعتماد، false = رفض (بدون سبب في V1 حسب القرار المعتمد)
        public bool Approve { get; set; }
    }

    public class UpdateDoctorDto
    {
        public int? SpecialtyId { get; set; }
        public string? Bio { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool? IsActive { get; set; }
    }
}
