using SmartTabbibk.Application.DTOs.Appointments;
using SmartTabbibk.Application.Features.Appointments;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Tests.Fakes;
using Xunit;

namespace SmartTabbibk.Tests.UnitTests
{
    /// <summary>
    /// Unit Tests لـ AppointmentService — تغطي أهم Business Rules في المشروع:
    /// فحص اعتماد الطبيب، فحص تعارض المواعيد، وقواعد الانتقال بين الحالات.
    /// كل اختبار يستخدم Fake Repositories (In-Memory) بدل قاعدة بيانات حقيقية،
    /// عشان نختبر منطق العمل بمعزل تام عن أي تفصيلة تقنية خارجية.
    /// </summary>
    public class AppointmentServiceTests
    {
        private static (AppointmentService service, FakeAppointmentRepository apptRepo,
            FakeDoctorRepository doctorRepo, FakePatientRepository patientRepo) CreateService()
        {
            var apptRepo = new FakeAppointmentRepository();
            var doctorRepo = new FakeDoctorRepository();
            var patientRepo = new FakePatientRepository();
            var notificationRepo = new FakeNotificationRepository();
            var priorityEvaluator = new FakePriorityEvaluator();

            var service = new AppointmentService(apptRepo, doctorRepo, patientRepo, notificationRepo, priorityEvaluator);
            return (service, apptRepo, doctorRepo, patientRepo);
        }

        [Fact]
        public async Task BookAsync_ShouldThrow_WhenDoctorNotApproved()
        {
            // Arrange: طبيب غير معتمد
            var (service, _, doctorRepo, patientRepo) = CreateService();
            patientRepo.Patients.Add(new Patient { PatientId = 1, UserId = 10 });
            doctorRepo.Doctors.Add(new Doctor { DoctorId = 1, IsApproved = false });

            var dto = new BookAppointmentDto { DoctorId = 1, AppointmentDate = DateTime.UtcNow.AddDays(1) };

            // Act + Assert: المفروض يرمي استثناء برسالة واضحة
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.BookAsync(dto, patientUserId: 10));
            Assert.Contains("غير معتمد", ex.Message);
        }

        [Fact]
        public async Task BookAsync_ShouldThrow_WhenDoctorTimeConflict()
        {
            // Arrange: طبيب معتمد، لكن نفس الوقت محجوز بالفعل
            var (service, apptRepo, doctorRepo, patientRepo) = CreateService();
            var date = DateTime.UtcNow.AddDays(1);

            patientRepo.Patients.Add(new Patient { PatientId = 1, UserId = 10 });
            doctorRepo.Doctors.Add(new Doctor { DoctorId = 1, IsApproved = true });
            apptRepo.Appointments.Add(new Appointment
            {
                AppointmentId = 99, DoctorId = 1, PatientId = 2,
                AppointmentDate = date, Status = AppointmentStatus.Pending
            });

            var dto = new BookAppointmentDto { DoctorId = 1, AppointmentDate = date };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.BookAsync(dto, patientUserId: 10));
            Assert.Contains("محجوز", ex.Message);
        }

        [Fact]
        public async Task BookAsync_ShouldThrow_WhenPatientAlreadyHasAppointmentAtSameTime()
        {
            // Arrange: نفس المريض عنده حجز آخر بنفس التوقيت (مع طبيب مختلف)
            var (service, apptRepo, doctorRepo, patientRepo) = CreateService();
            var date = DateTime.UtcNow.AddDays(1);

            patientRepo.Patients.Add(new Patient { PatientId = 1, UserId = 10 });
            doctorRepo.Doctors.Add(new Doctor { DoctorId = 1, IsApproved = true });
            doctorRepo.Doctors.Add(new Doctor { DoctorId = 2, IsApproved = true });
            apptRepo.Appointments.Add(new Appointment
            {
                AppointmentId = 99, DoctorId = 2, PatientId = 1,
                AppointmentDate = date, Status = AppointmentStatus.Pending
            });

            var dto = new BookAppointmentDto { DoctorId = 1, AppointmentDate = date };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.BookAsync(dto, patientUserId: 10));
            Assert.Contains("حجز آخر", ex.Message);
        }

        [Fact]
        public async Task BookAsync_ShouldSucceed_WhenNoConflictsAndDoctorApproved()
        {
            // Arrange: كل الشروط سليمة
            var (service, apptRepo, doctorRepo, patientRepo) = CreateService();
            patientRepo.Patients.Add(new Patient { PatientId = 1, UserId = 10 });
            doctorRepo.Doctors.Add(new Doctor { DoctorId = 1, IsApproved = true });

            var dto = new BookAppointmentDto
            {
                DoctorId = 1,
                AppointmentDate = DateTime.UtcNow.AddDays(1),
                Symptoms = "صداع",
                Priority = "Normal"
            };

            var result = await service.BookAsync(dto, patientUserId: 10);

            Assert.Equal("Pending", result.Status);
            Assert.Single(apptRepo.Appointments);
        }

        [Theory]
        [InlineData("Pending", "Confirmed", true)]
        [InlineData("Pending", "Cancelled", true)]
        [InlineData("Confirmed", "Completed", true)]
        [InlineData("Confirmed", "Cancelled", true)]
        [InlineData("Pending", "Completed", false)]   // قفز غير مسموح
        [InlineData("Completed", "Pending", false)]   // رجوع للخلف غير منطقي وغير مسموح
        [InlineData("Cancelled", "Confirmed", false)]  // موعد ملغي لا يُعاد إحياؤه
        public async Task UpdateStatusAsync_ShouldRespectAllowedTransitionsOnly(
            string currentStatus, string newStatus, bool shouldSucceed)
        {
            // Arrange
            var (service, apptRepo, doctorRepo, patientRepo) = CreateService();
            doctorRepo.Doctors.Add(new Doctor { DoctorId = 1, UserId = 100, IsApproved = true });
            // لازم مريض حقيقي في الـ Fake عشان AppointmentService بيبعت إشعار له بعد أي تحديث ناجح للحالة
            patientRepo.Patients.Add(new Patient { PatientId = 1, UserId = 20 });
            apptRepo.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                DoctorId = 1,
                PatientId = 1,
                Status = Enum.Parse<AppointmentStatus>(currentStatus),
                AppointmentDate = DateTime.UtcNow.AddDays(1)
            });

            var dto = new UpdateStatusDto { Status = newStatus };

            // Act + Assert
            if (shouldSucceed)
            {
                await service.UpdateStatusAsync(1, dto, doctorUserId: 100);
                Assert.Equal(newStatus, apptRepo.Appointments[0].Status.ToString());
            }
            else
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => service.UpdateStatusAsync(1, dto, doctorUserId: 100));
            }
        }
    }
}
