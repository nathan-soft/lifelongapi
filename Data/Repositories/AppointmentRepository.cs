using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using static LifeLongApi.Codes.AppHelper;

namespace LifeLongApi.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<List<Appointment>> GetAllMentorAppointmentsAsync(int mentorId);
        Task<List<Appointment>> GetMentorAppointmentsByTypeAsync(int mentorId, AppointmentStatus status);
    }

    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(IdentityAppContext context) : base(context)
        {

        }

        public async Task<List<Appointment>> GetAllMentorAppointmentsAsync(int mentorId)
        {
            return await context.Set<Appointment>()
                                .Where(a => a.MentorId == mentorId)
                                .ToListAsync();
        }

        public async Task<List<Appointment>> GetMentorAppointmentsByTypeAsync(int mentorId, AppointmentStatus status)
        {
            IQueryable<Appointment> userAppointments;
           
            if(status == AppointmentStatus.PENDING)
            {
                //get both pending and postponed appointment(s) as they are the same thing.
                userAppointments = context.Set<Appointment>()
                                .Where(a => a.MentorId == mentorId
                                            && a.Status == status.ToString()
                                            || a.Status == AppointmentStatus.POSTPONED.ToString());
            }
            else
            {
                userAppointments = context.Set<Appointment>()
                               .Where(a => a.MentorId == mentorId
                                           && a.Status == status.ToString());
            }
            
            return await userAppointments.ToListAsync();
        }
    }
}