using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Domain.Interfaces
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        Task<MedicalRecord?> GetByAppointmentIdAsync(int appointmentId);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
    }
}
