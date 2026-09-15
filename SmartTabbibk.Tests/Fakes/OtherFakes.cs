using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Tests.Fakes
{
    public class FakeDoctorRepository : IDoctorRepository
    {
        public List<Doctor> Doctors { get; } = new();

        public Task<Doctor?> GetByIdAsync(int id) =>
            Task.FromResult(Doctors.FirstOrDefault(d => d.DoctorId == id));

        public Task<IEnumerable<Doctor>> GetAllAsync() => Task.FromResult(Doctors.AsEnumerable());
        public Task AddAsync(Doctor entity) { Doctors.Add(entity); return Task.CompletedTask; }
        public void Update(Doctor entity) { }
        public void Delete(Doctor entity) => Doctors.Remove(entity);
        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task<IEnumerable<Doctor>> SearchAsync(string? specialty, string? name, decimal? minPrice, decimal? maxPrice, double? minRating) =>
            Task.FromResult(Doctors.AsEnumerable());

        public Task<Doctor?> GetWithDetailsAsync(int doctorId) => GetByIdAsync(doctorId);
        public Task<Doctor?> GetByUserIdAsync(int userId) =>
            Task.FromResult(Doctors.FirstOrDefault(d => d.UserId == userId));

        public Task<IEnumerable<Doctor>> GetPendingApprovalAsync() =>
            Task.FromResult(Doctors.Where(d => !d.IsApproved));
    }

    public class FakePatientRepository : IPatientRepository
    {
        public List<Patient> Patients { get; } = new();

        public Task<Patient?> GetByIdAsync(int id) =>
            Task.FromResult(Patients.FirstOrDefault(p => p.PatientId == id));

        public Task<IEnumerable<Patient>> GetAllAsync() => Task.FromResult(Patients.AsEnumerable());
        public Task AddAsync(Patient entity) { Patients.Add(entity); return Task.CompletedTask; }
        public void Update(Patient entity) { }
        public void Delete(Patient entity) => Patients.Remove(entity);
        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task<Patient?> GetByUserIdAsync(int userId) =>
            Task.FromResult(Patients.FirstOrDefault(p => p.UserId == userId));
    }

    public class FakeNotificationRepository : INotificationRepository
    {
        public List<Notification> Notifications { get; } = new();

        public Task<Notification?> GetByIdAsync(int id) =>
            Task.FromResult(Notifications.FirstOrDefault(n => n.NotificationId == id));

        public Task<IEnumerable<Notification>> GetAllAsync() => Task.FromResult(Notifications.AsEnumerable());
        public Task AddAsync(Notification entity) { Notifications.Add(entity); return Task.CompletedTask; }
        public void Update(Notification entity) { }
        public void Delete(Notification entity) => Notifications.Remove(entity);
        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task<IEnumerable<Notification>> GetByUserIdAsync(int userId) =>
            Task.FromResult(Notifications.Where(n => n.UserId == userId));
    }

    /// <summary>
    /// تنفيذ ثابت (Deterministic) لـ IPriorityEvaluator يرجّع قيمة معروفة مسبقاً،
    /// عشان نختبر AppointmentService من غير ما نعتمد على منطق حقيقي هنا
    /// </summary>
    public class FakePriorityEvaluator : IPriorityEvaluator
    {
        public PriorityLevel Evaluate(string symptoms) => PriorityLevel.Normal;
    }

    public class FakeMedicineRepository : IMedicineRepository
    {
        public List<Medicine> Medicines { get; } = new();

        public Task<Medicine?> GetByIdAsync(int id) =>
            Task.FromResult(Medicines.FirstOrDefault(m => m.MedicineId == id));

        public Task<IEnumerable<Medicine>> GetAllAsync() => Task.FromResult(Medicines.AsEnumerable());
        public Task AddAsync(Medicine entity) { Medicines.Add(entity); return Task.CompletedTask; }
        public void Update(Medicine entity) { }
        public void Delete(Medicine entity) => Medicines.Remove(entity);
        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task<IEnumerable<Medicine>> GetActiveAsync() =>
            Task.FromResult(Medicines.Where(m => m.IsActive));

        public Task<bool> IsUsedInPrescriptionAsync(int medicineId) => Task.FromResult(false);

        public Task<bool> ExistsAsync(string name, string strength, string form) =>
            Task.FromResult(Medicines.Any(m => m.Name == name && m.Strength == strength && m.Form == form));
    }
}
