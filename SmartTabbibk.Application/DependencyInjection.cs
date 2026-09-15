using Microsoft.Extensions.DependencyInjection;
using SmartTabbibk.Application.Features.Appointments;
using SmartTabbibk.Application.Features.Auth;
using SmartTabbibk.Application.Features.Doctors;
using SmartTabbibk.Application.Features.MedicalRecords;
using SmartTabbibk.Application.Features.Medicines;
using SmartTabbibk.Application.Features.Prescriptions;
using SmartTabbibk.Application.Features.Reviews;
using SmartTabbibk.Application.Features.Specialties;
using SmartTabbibk.Application.Interfaces;

namespace SmartTabbibk.Application
{
    /// <summary>
    /// يُستدعى من Program.cs:
    /// builder.Services.AddApplication();
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IMedicineService, MedicineService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<IReviewService, ReviewService>();
            // باقي الـ Services (Notifications, Reports...) تُضاف هنا لاحقاً بنفس النمط

            return services;
        }
    }
}
