using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(AppDbContext context) : base(context) { }

        public async Task<MedicalRecord?> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _dbSet
                .Include(m => m.Prescriptions).ThenInclude(p => p.Medicine)
                .FirstOrDefaultAsync(m => m.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _dbSet
                .Include(m => m.Appointment).ThenInclude(a => a.Doctor).ThenInclude(d => d.User)
                .Include(m => m.Prescriptions).ThenInclude(p => p.Medicine)
                .Where(m => m.Appointment.PatientId == patientId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
