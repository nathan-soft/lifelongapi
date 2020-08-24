using System.Threading.Tasks;
using LifeLongApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LifeLongApi.Data.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        public Task<List<Notification>> GetNotificationsForUserAsync(int userId);
    }

    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IdentityAppContext context) : base(context) { }

        /*
        *gets notifications for the user it was craeted for.
        */
        public async Task<List<Notification>> GetNotificationsForUserAsync(int createdForUserId)
        {
            return await context.Set<Notification>()
                                .Where(n => n.CreatedForId == createdForUserId)
                                .ToListAsync();
        }
    }
}