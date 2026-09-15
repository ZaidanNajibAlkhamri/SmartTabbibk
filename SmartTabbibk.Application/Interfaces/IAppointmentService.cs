using SmartTabbibk.Application.DTOs.Appointments;

namespace SmartTabbibk.Application.Interfaces
{
    public interface IAppointmentService
    {
        // patientUserId: الـ UserId من الـ JWT — الخدمة تحوّله لـ PatientId داخلياً
        Task<AppointmentResponseDto> BookAsync(BookAppointmentDto dto, int patientUserId);

        Task CancelAsync(int appointmentId, int patientUserId);

        Task<IEnumerable<AppointmentResponseDto>> GetMineAsync(int userId, string role);

        Task<AppointmentResponseDto> GetByIdAsync(int appointmentId, int userId, string role);

        Task UpdatePriorityAsync(int appointmentId, UpdatePriorityDto dto, int doctorUserId);

        Task UpdateStatusAsync(int appointmentId, UpdateStatusDto dto, int doctorUserId);
    }
}
