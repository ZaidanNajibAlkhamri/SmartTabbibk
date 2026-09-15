using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;

namespace SmartTabbibk.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Specialty> Specialties => Set<Specialty>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== تحديد صريح للمفتاح الأساسي (RecordId لا يطابق اصطلاح EF التلقائي) =====
            modelBuilder.Entity<MedicalRecord>()
                .HasKey(m => m.RecordId);

            // ===== Business Rule: البريد الإلكتروني فريد =====
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ===== Business Rule: تركيبة الدواء فريدة =====
            modelBuilder.Entity<Medicine>()
                .HasIndex(m => new { m.Name, m.Strength, m.Form })
                .IsUnique();

            // ===== Business Rule: تقييم واحد لكل مريض لكل موعد =====
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.PatientId, r.AppointmentId })
                .IsUnique();

            // ===== Appointment ↔ Patient / Doctor =====
            // Restrict بدل Cascade لتجنب تعارض المسارات (User له مسارين لـ Appointment عبر Patient وDoctor)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Review ↔ Patient / Doctor (نفس مشكلة تعدد المسارات) =====
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== User ↔ Patient / Doctor (1:1 اختيارية) =====
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId);

            // ===== Appointment ↔ MedicalRecord (1:1) =====
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(m => m.AppointmentId);

            // ===== MedicalRecord ↔ Prescription (نفس مشكلة RecordId مع الاصطلاح التلقائي) =====
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.MedicalRecord)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.RecordId);

            // ===== Appointment ↔ Review (1:1 اختيارية) =====
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Appointment)
                .WithOne(a => a.Review)
                .HasForeignKey<Review>(r => r.AppointmentId);

            // ===== Decimal precision لسعر الكشف =====
            modelBuilder.Entity<Doctor>()
                .Property(d => d.ConsultationFee)
                .HasPrecision(10, 2);
        }
    }
}
