using SmartTabbibk.Application.DTOs.Prescriptions;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Application.Features.Prescriptions
{
    /// <summary>
    /// تنفيذ UC16 — إنشاء وصفة طبية
    /// نفس منطق Sequence Diagram المعتمد: حلقة على كل دواء، تجاهل غير النشط مع تحذير
    /// </summary>
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMedicalRecordRepository _recordRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IMedicalRecordRepository recordRepository,
            IMedicineRepository medicineRepository,
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _recordRepository = recordRepository;
            _medicineRepository = medicineRepository;
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<PrescriptionResponseDto> CreateAsync(CreatePrescriptionDto dto, int doctorUserId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(doctorUserId)
                ?? throw new InvalidOperationException("حساب الطبيب غير موجود.");

            var record = await _recordRepository.GetByIdAsync(dto.RecordId)
                ?? throw new InvalidOperationException("السجل الطبي غير موجود.");

            // فحص الملكية: السجل الطبي يخص موعداً لهذا الطبيب
            var appointment = await _appointmentRepository.GetByIdAsync(record.AppointmentId)
                ?? throw new InvalidOperationException("الموعد المرتبط بالسجل غير موجود.");

            if (appointment.DoctorId != doctor.DoctorId)
                throw new UnauthorizedAccessException("هذا السجل الطبي لا يخص هذا الطبيب.");

            var response = new PrescriptionResponseDto { RecordId = dto.RecordId };

            foreach (var item in dto.Items)
            {
                var medicine = await _medicineRepository.GetByIdAsync(item.MedicineId);

                if (medicine == null || !medicine.IsActive)
                {
                    response.Warnings.Add(
                        $"تم تجاهل الدواء (MedicineId={item.MedicineId}) لأنه غير موجود أو غير نشط.");
                    continue;
                }

                var prescription = new Prescription
                {
                    RecordId = dto.RecordId,
                    MedicineId = item.MedicineId,
                    Dosage = item.Dosage,
                    Duration = item.Duration,
                    Instructions = item.Instructions
                };

                await _prescriptionRepository.AddAsync(prescription);

                response.SavedItems.Add(new PrescriptionItemResultDto
                {
                    MedicineName = medicine.Name,
                    Dosage = item.Dosage,
                    Duration = item.Duration,
                    Instructions = item.Instructions
                });
            }

            await _prescriptionRepository.SaveChangesAsync();

            return response;
        }

        public async Task<IEnumerable<PrescriptionItemResultDto>> GetByRecordIdAsync(int recordId)
        {
            var prescriptions = await _prescriptionRepository.GetByRecordIdAsync(recordId);

            return prescriptions.Select(p => new PrescriptionItemResultDto
            {
                PrescriptionId = p.PrescriptionId,
                MedicineName = p.Medicine.Name,
                Dosage = p.Dosage,
                Duration = p.Duration,
                Instructions = p.Instructions
            }).ToList();
        }
    }
}
