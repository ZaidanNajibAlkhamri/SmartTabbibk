namespace SmartTabbibk.Domain.Entities
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }

        public int RecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; } = null!;

        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; } = null!;

        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Instructions { get; set; }
    }
}
