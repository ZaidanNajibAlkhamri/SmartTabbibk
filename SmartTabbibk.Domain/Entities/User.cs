using SmartTabbibk.Domain.Enums;

namespace SmartTabbibk.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (علاقة 1:1 اختيارية حسب الدور)
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
