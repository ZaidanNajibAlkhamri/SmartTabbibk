namespace SmartTabbibk.Domain.Entities
{
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;

        // false = معطّل، لا يظهر في وصفات جديدة، لكن يبقى بالسجلات القديمة
        public bool IsActive { get; set; } = true;

        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        // UNIQUE(Name, Strength, Form) يُطبَّق في Infrastructure (EF Core Fluent API)
    }
}
