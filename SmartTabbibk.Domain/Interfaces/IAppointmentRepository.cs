using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);

        /// <summary>
        /// يفحص Business Rule: منع حجزين لنفس الطبيب في نفس التوقيت
        /// </summary>
        Task<bool> HasConflictAsync(int doctorId, DateTime appointmentDate);

        /// <summary>
        /// يفحص Business Rule: منع المريض من حجز موعدين في نفس التوقيت
        /// </summary>
        Task<bool> PatientHasConflictAsync(int patientId, DateTime appointmentDate);

        Task<Appointment?> GetWithDetailsAsync(int appointmentId);
    }
}
