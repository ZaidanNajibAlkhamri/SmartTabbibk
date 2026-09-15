using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartTabbibk.Application.Interfaces;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;
using SmartTabbibk.Infrastructure.Repositories;
using SmartTabbibk.Infrastructure.Services;

namespace SmartTabbibk.Infrastructure
{
    /// <summary>
    /// يُستدعى مرة واحدة من Program.cs:
    /// builder.Services.AddInfrastructure(builder.Configuration);
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IMedicineRepository, MedicineRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            // باقي الـ Repositories (Notification موجودة أصلاً) تُضاف هنا لاحقاً بنفس النمط

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            // ===== Strategy Pattern الفعلي: اختيار التنفيذ حسب appsettings.json =====
            // "AI:UseAIPriorityEvaluator": false → ManualPriorityEvaluator (V1، الافتراضي)
            // "AI:UseAIPriorityEvaluator": true  → AIPriorityEvaluator (V2، نموذج ML.NET مدرَّب)
            // نفس AppointmentService بالضبط، بدون أي تعديل — هذا هو جوهر Open/Closed Principle
            if (configuration.GetValue<bool>("AI:UseAIPriorityEvaluator"))
            {
                services.AddSingleton<PriorityModelService>();
                services.AddScoped<IPriorityEvaluator, AIPriorityEvaluator>();
            }
            else
            {
                services.AddScoped<IPriorityEvaluator, ManualPriorityEvaluator>();
            }

            return services;
        }
    }
}
