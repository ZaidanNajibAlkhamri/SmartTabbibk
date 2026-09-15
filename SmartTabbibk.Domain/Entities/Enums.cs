namespace SmartTabbibk.Domain.Enums
{
    public enum UserRole
    {
        Patient,
        Doctor,
        Admin
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }

    public enum PriorityLevel
    {
        Normal,
        Urgent
    }

    public enum NotificationType
    {
        AppointmentBooked,
        AppointmentConfirmed,
        AppointmentCancelled,
        AppointmentReminder,
        DoctorApproved,
        DoctorRejected
    }
}
