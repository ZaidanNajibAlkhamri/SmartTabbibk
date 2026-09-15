using Microsoft.EntityFrameworkCore;
using SmartTabbibk.Domain.Entities;
using SmartTabbibk.Domain.Interfaces;
using SmartTabbibk.Infrastructure.Data;

namespace SmartTabbibk.Infrastructure.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
