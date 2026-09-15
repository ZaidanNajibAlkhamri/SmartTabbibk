using SmartTabbibk.Application.DTOs.MedicalRecords;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.MedicalRecords
{
    /// <summary>
    /// تنفيذ UC15 (كتابة تشخيص)، UC7 (سجلي الطبي)، UC25 (عرض إداري Read Only)
    /// </summary>
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _recordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;

        public MedicalRecordService(
            IMedicalRecordRepository recordRepository,
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository)
        {
            _recordRepository = recordRepository;
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }

        public async Task<MedicalRecordResponseDto> WriteDiagnosisAsync(WriteDiagnosisDto dto, int doctorUserId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(doctorUserId)
                ?? throw new InvalidOperationException("حساب الطبيب غير موجود.");

            var appointment = await _appointmentRepository.GetWithDetailsAsync(dto.AppointmentId)
                ?? throw new KeyNotFoundException("الموعد غير موجود.");

            // فحص: الموعد يخص هذا الطبيب
            if (appointment.DoctorId != doctor.DoctorId)
                throw new UnauthorizedAccessException("هذا الموعد لا يخص هذا الطبيب.");

            // فحص UC15 المعتمد: لازم يكون الموعد Confirmed قبل كتابة التشخيص
            if (appointment.Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("لا يمكن كتابة تشخيص إلا لموعد بحالة Confirmed.");

            var record = new MedicalRecord
            {
                AppointmentId = dto.AppointmentId,
                Diagnosis = dto.Diagnosis,
                Notes = dto.Notes
            };

            await _recordRepository.AddAsync(record);

            // نفس التدفق المعتمد: كتابة تشخيص ← تحويل الموعد إلى Completed ← في نفس العملية
            appointment.Status = AppointmentStatus.Completed;
            _appointmentRepository.Update(appointment);

            // حفظ الاثنين معاً (نفس AppDbContext، نفس SaveChanges)
            await _recordRepository.SaveChangesAsync();

            var saved = await _recordRepository.GetByAppointmentIdAsync(dto.AppointmentId);
            return MapToDto(saved!, appointment);
        }

        public async Task<MedicalRecordResponseDto> GetByAppointmentIdAsync(int appointmentId, int userId, string role)
        {
            var appointment = await _appointmentRepository.GetWithDetailsAsync(appointmentId)
                ?? throw new KeyNotFoundException("الموعد غير موجود.");

            // فحص الملكية: فقط المريض أو الطبيب الخاص بهذا الموعد
            if (role == "Patient")
            {
                var patient = await _patientRepository.GetByUserIdAsync(userId);
                if (patient == null || appointment.PatientId != patient.PatientId)
                    throw new UnauthorizedAccessException("لا تملك صلاحية عرض هذا السجل.");
            }
            else if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);
                if (doctor == null || appointment.DoctorId != doctor.DoctorId)
                    throw new UnauthorizedAccessException("لا تملك صلاحية عرض هذا السجل.");
            }

            var record = await _recordRepository.GetByAppointmentIdAsync(appointmentId)
                ?? throw new KeyNotFoundException("لا يوجد سجل طبي لهذا الموعد بعد.");

            return MapToDto(record, appointment);
        }

        public async Task<IEnumerable<MedicalRecordResponseDto>> GetMineAsync(int patientUserId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(patientUserId)
                ?? throw new InvalidOperationException("حساب المريض غير موجود.");

            var records = await _recordRepository.GetByPatientIdAsync(patient.PatientId);

            return records.Select(r => MapToDto(r, r.Appointment)).ToList();
        }

        public async Task<MedicalRecordResponseDto> GetForAdminAsync(int recordId)
        {
            // UC25: عرض إداري Read Only — لا فحص ملكية، Admin يشوف أي سجل
            var record = await _recordRepository.GetByIdAsync(recordId)
                ?? throw new KeyNotFoundException("السجل الطبي غير موجود.");

            var fullRecord = await _recordRepository.GetByAppointmentIdAsync(record.AppointmentId)
                ?? record;
            var appointment = await _appointmentRepository.GetWithDetailsAsync(record.AppointmentId);

            return MapToDto(fullRecord, appointment!);
        }

        private static MedicalRecordResponseDto MapToDto(MedicalRecord record, Appointment appointment) => new()
        {
            RecordId = record.RecordId,
            AppointmentId = record.AppointmentId,
            PatientName = appointment.Patient?.User?.Name ?? string.Empty,
            DoctorName = appointment.Doctor?.User?.Name ?? string.Empty,
            Diagnosis = record.Diagnosis,
            Notes = record.Notes,
            CreatedAt = record.CreatedAt,
            Prescriptions = record.Prescriptions?.Select(p => new PrescriptionSummaryDto
            {
                MedicineName = p.Medicine?.Name ?? string.Empty,
                Dosage = p.Dosage,
                Duration = p.Duration,
                Instructions = p.Instructions
            }).ToList() ?? new List<PrescriptionSummaryDto>()
        };
    }
}
