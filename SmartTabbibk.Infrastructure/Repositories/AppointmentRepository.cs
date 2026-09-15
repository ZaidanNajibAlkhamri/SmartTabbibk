using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _dbSet
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                // Business Rule: المواعيد المستعجلة تظهر أولاً
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.Priority)
                .ThenBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<bool> HasConflictAsync(int doctorId, DateTime appointmentDate)
        {
            return await _dbSet.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == appointmentDate &&
                a.Status != Domain.Enums.AppointmentStatus.Cancelled);
        }

        public async Task<bool> PatientHasConflictAsync(int patientId, DateTime appointmentDate)
        {
            return await _dbSet.AnyAsync(a =>
                a.PatientId == patientId &&
                a.AppointmentDate == appointmentDate &&
                a.Status != Domain.Enums.AppointmentStatus.Cancelled);
        }

        public async Task<Appointment?> GetWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }
    }
}
