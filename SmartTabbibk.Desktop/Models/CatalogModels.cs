namespace SmartTabbibk.Desktop.Models
{
    public class MedicineDto
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateMedicineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
    }

    public class SpecialtyDto
    {
        public int SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateSpecialtyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Keywords { get; set; }
    }
}
