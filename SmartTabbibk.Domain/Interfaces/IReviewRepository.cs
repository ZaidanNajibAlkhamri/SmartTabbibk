using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByDoctorIdAsync(int doctorId);
        Task<bool> ExistsForAppointmentAsync(int patientId, int appointmentId);
    }
}
