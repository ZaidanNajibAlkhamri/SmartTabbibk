using SmartTabbibk.Application.DTOs.MedicalRecords;

namespace SmartTabbibk.Application.Interfaces
{
    public interface IMedicalRecordService
    {
        // doctorUserId: الـ UserId من الـ JWT
        Task<MedicalRecordResponseDto> WriteDiagnosisAsync(WriteDiagnosisDto dto, int doctorUserId);

        Task<MedicalRecordResponseDto> GetByAppointmentIdAsync(int appointmentId, int userId, string role);

        // UC7 — سجلي الطبي الكامل — Patient
        Task<IEnumerable<MedicalRecordResponseDto>> GetMineAsync(int patientUserId);

        // UC25 — عرض للإدارة Read Only — Admin
        Task<MedicalRecordResponseDto> GetForAdminAsync(int recordId);
    }
}
