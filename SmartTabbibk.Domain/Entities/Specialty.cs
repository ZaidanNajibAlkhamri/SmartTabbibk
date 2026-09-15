namespace SmartTabbibk.Domain.Entities
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;

        // كلمات مفتاحية تُستخدم مستقبلاً من AI Priority/Specialty Evaluator
        public string? Keywords { get; set; }

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
