using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Tests.Fakes
{
    /// <summary>
    /// تنفيذ وهمي (In-Memory) لـ IAppointmentRepository يستخدم في الاختبارات
    /// بدلاً من الاتصال بقاعدة بيانات حقيقية — هذا هو جوهر الـ Unit Testing:
    /// عزل منطق العمل (Business Logic) عن التفاصيل التقنية (EF Core/SQL Server)
    /// </summary>
    public class FakeAppointmentRepository : IAppointmentRepository
    {
        public List<Appointment> Appointments { get; } = new();
        private int _nextId = 1;

        public Task<Appointment?> GetByIdAsync(int id) =>
            Task.FromResult(Appointments.FirstOrDefault(a => a.AppointmentId == id));

        public Task<IEnumerable<Appointment>> GetAllAsync() =>
            Task.FromResult(Appointments.AsEnumerable());

        public Task AddAsync(Appointment entity)
        {
            entity.AppointmentId = _nextId++;
            Appointments.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(Appointment entity) { /* In-memory، التعديل مباشر على الكائن نفسه */ }

        public void Delete(Appointment entity) => Appointments.Remove(entity);

        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId) =>
            Task.FromResult(Appointments.Where(a => a.PatientId == patientId));

        public Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId) =>
            Task.FromResult(Appointments.Where(a => a.DoctorId == doctorId));

        public Task<bool> HasConflictAsync(int doctorId, DateTime appointmentDate) =>
            Task.FromResult(Appointments.Any(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == appointmentDate &&
                a.Status != AppointmentStatus.Cancelled));

        public Task<bool> PatientHasConflictAsync(int patientId, DateTime appointmentDate) =>
            Task.FromResult(Appointments.Any(a =>
                a.PatientId == patientId &&
                a.AppointmentDate == appointmentDate &&
                a.Status != AppointmentStatus.Cancelled));

        public Task<Appointment?> GetWithDetailsAsync(int appointmentId) => GetByIdAsync(appointmentId);
    }
}
