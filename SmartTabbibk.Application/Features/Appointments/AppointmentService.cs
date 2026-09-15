using SmartTabbibk.Application.DTOs.Appointments;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPriorityEvaluator _priorityEvaluator;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            INotificationRepository notificationRepository,
            IPriorityEvaluator priorityEvaluator)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _notificationRepository = notificationRepository;
            _priorityEvaluator = priorityEvaluator;
        }

        public async Task<AppointmentResponseDto> BookAsync(BookAppointmentDto dto, int patientUserId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(patientUserId)
                ?? throw new InvalidOperationException("حساب المريض غير موجود.");

            // فحص 1: الطبيب معتمد
            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId)
                ?? throw new InvalidOperationException("الطبيب غير موجود.");

            if (!doctor.IsApproved)
                throw new InvalidOperationException("الطبيب غير معتمد.");

            // فحص 2: تعارض وقت الطبيب
            if (await _appointmentRepository.HasConflictAsync(dto.DoctorId, dto.AppointmentDate))
                throw new InvalidOperationException("الوقت محجوز مسبقاً لهذا الطبيب.");

            // فحص 3: تعارض حجز المريض نفسه
            if (await _appointmentRepository.PatientHasConflictAsync(patient.PatientId, dto.AppointmentDate))
                throw new InvalidOperationException("لديك حجز آخر بنفس الوقت.");

            // تحديد الأولوية: المريض يحدد يدوياً، وإلا Fallback عبر IPriorityEvaluator (Strategy Pattern)
            var priority = ParsePriorityOrDefault(dto.Priority, dto.Symptoms);

            var appointment = new Appointment
            {
                PatientId = patient.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                Symptoms = dto.Symptoms,
                Priority = priority,
                Status = AppointmentStatus.Pending
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            // إشعار تلقائي للطبيب (Business Rule)
            await _notificationRepository.AddAsync(new Notification
            {
                UserId = doctor.UserId,
                Message = "لديك حجز موعد جديد.",
                Type = NotificationType.AppointmentBooked
            });
            await _notificationRepository.SaveChangesAsync();

            var saved = await _appointmentRepository.GetWithDetailsAsync(appointment.AppointmentId);
            return MapToDto(saved!);
        }

        public async Task CancelAsync(int appointmentId, int patientUserId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(patientUserId)
                ?? throw new InvalidOperationException("حساب المريض غير موجود.");

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId)
                ?? throw new KeyNotFoundException("الموعد غير موجود.");

            if (appointment.PatientId != patient.PatientId)
                throw new UnauthorizedAccessException("هذا الموعد لا يخص هذا الحساب.");

            // Business Rule: لا إلغاء لو أقل من ساعة على الموعد
            if (appointment.AppointmentDate - DateTime.UtcNow < TimeSpan.FromHours(1))
                throw new InvalidOperationException("لا يمكن الإلغاء قبل أقل من ساعة من الموعد.");

            appointment.Status = AppointmentStatus.Cancelled;
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();

            await _notificationRepository.AddAsync(new Notification
            {
                UserId = (await _doctorRepository.GetByIdAsync(appointment.DoctorId))!.UserId,
                Message = "تم إلغاء موعد من قبل المريض.",
                Type = NotificationType.AppointmentCancelled
            });
            await _notificationRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetMineAsync(int userId, string role)
        {
            IEnumerable<Appointment> appointments;

            if (role == "Patient")
            {
                var patient = await _patientRepository.GetByUserIdAsync(userId)
                    ?? throw new InvalidOperationException("حساب المريض غير موجود.");
                appointments = await _appointmentRepository.GetByPatientIdAsync(patient.PatientId);
            }
            else if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId)
                    ?? throw new InvalidOperationException("حساب الطبيب غير موجود.");
                appointments = await _appointmentRepository.GetByDoctorIdAsync(doctor.DoctorId);
            }
            else
            {
                throw new UnauthorizedAccessException("هذا الدور لا يملك مواعيد خاصة به.");
            }

            return appointments.Select(MapToDto).ToList();
        }

        public async Task<AppointmentResponseDto> GetByIdAsync(int appointmentId, int userId, string role)
        {
            var appointment = await _appointmentRepository.GetWithDetailsAsync(appointmentId)
                ?? throw new KeyNotFoundException("الموعد غير موجود.");

            // فحص الملكية: فقط المريض أو الطبيب الخاص بهذا الموعد
            if (role == "Patient")
            {
                var patient = await _patientRepository.GetByUserIdAsync(userId);
                if (patient == null || appointment.PatientId != patient.PatientId)
                    throw new UnauthorizedAccessException("لا تملك صلاحية عرض هذا الموعد.");
            }
            else if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);
                if (doctor == null || appointment.DoctorId != doctor.DoctorId)
                    throw new UnauthorizedAccessException("لا تملك صلاحية عرض هذا الموعد.");
            }

            return MapToDto(appointment);
        }

        public async Task UpdatePriorityAsync(int appointmentId, UpdatePriorityDto dto, int doctorUserId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(doctorUserId)
                ?? throw new InvalidOperationException("حساب الطبيب غير موجود.");

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId)
                ?? throw new KeyNotFoundException("الموعد غير موجود.");

            if (appointment.DoctorId != doctor.DoctorId)
                throw new UnauthorizedAccessException("هذا الموعد لا يخص هذا الطبيب.");

            if (!Enum.TryParse<PriorityLevel>(dto.Priority, true, out var priority))
                throw new InvalidOperationException("قيمة الأولوية غير صحيحة.");

            appointment.Priority = priority;
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int appointmentId, UpdateStatusDto dto, int doctorUserId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(doctorUserId)
                ?? throw new InvalidOperationException("حساب الطبيب غير موجود.");

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId)
                ?? throw new KeyNotFoundException("الموعد غير موجود.");

            if (appointment.DoctorId != doctor.DoctorId)
                throw new UnauthorizedAccessException("هذا الموعد لا يخص هذا الطبيب.");

            if (!Enum.TryParse<AppointmentStatus>(dto.Status, true, out var newStatus))
                throw new InvalidOperationException("قيمة الحالة غير صحيحة.");

            // Business Rule: مسار الحالات المسموح فقط
            // Pending → Confirmed → Completed  |  Pending → Cancelled
            var allowed = (appointment.Status, newStatus) switch
            {
                (AppointmentStatus.Pending, AppointmentStatus.Confirmed) => true,
                (AppointmentStatus.Pending, AppointmentStatus.Cancelled) => true,
                (AppointmentStatus.Confirmed, AppointmentStatus.Completed) => true,
                (AppointmentStatus.Confirmed, AppointmentStatus.Cancelled) => true,
                _ => false
            };

            if (!allowed)
                throw new InvalidOperationException(
                    $"لا يمكن تغيير حالة الموعد من {appointment.Status} إلى {newStatus}.");

            appointment.Status = newStatus;
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();

            await _notificationRepository.AddAsync(new Notification
            {
                UserId = (await _patientRepository.GetByIdAsync(appointment.PatientId))!.UserId,
                Message = $"تم تحديث حالة موعدك إلى {newStatus}.",
                Type = newStatus == AppointmentStatus.Cancelled
                    ? NotificationType.AppointmentCancelled
                    : NotificationType.AppointmentConfirmed
            });
            await _notificationRepository.SaveChangesAsync();
        }

        private PriorityLevel ParsePriorityOrDefault(string? priority, string? symptoms)
        {
            if (!string.IsNullOrWhiteSpace(priority) &&
                Enum.TryParse<PriorityLevel>(priority, true, out var parsed))
            {
                return parsed;
            }

            // Fallback عبر IPriorityEvaluator — V1: ManualPriorityEvaluator
            // V2 مستقبلاً: AIPriorityEvaluator بدون تغيير هذا الاستدعاء (Strategy Pattern)
            return _priorityEvaluator.Evaluate(symptoms ?? string.Empty);
        }

        private static AppointmentResponseDto MapToDto(Appointment a) => new()
        {
            AppointmentId = a.AppointmentId,
            PatientId = a.PatientId,
            PatientName = a.Patient?.User?.Name ?? string.Empty,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor?.User?.Name ?? string.Empty,
            SpecialtyName = a.Doctor?.Specialty?.Name ?? string.Empty,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status.ToString(),
            Priority = a.Priority.ToString(),
            Symptoms = a.Symptoms
        };
    }
}
