using LifeLongApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        /// <summary>
        /// Gets info of all mentees the mentor is currently training and the field of interests they're being trianed under.
        /// </summary>
        /// <param name="mentorId">The id of the mentor</param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a list that contains info about all mentees and field of interest they are being guided on by the mentor. 
        /// </returns>
        Task<List<Follow>> GetAllMentorshipInfoForMentor(int mentorId);

        /// <summary>
        /// Gets info that represents all mentors and field of interests belonging to each mentor that the mentee is currently receiving training under. 
        /// </summary>
        /// <param name="menteeId">the id of the mentee.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list that contains info about all mentors and field of interests that they are guiding the mentee on.
        /// </returns>
        Task<List<Follow>> GetAllMentorshipInfoForMentee(int menteeId);
        Task<List<Follow>> GetAllMentorshipRequestAsync();

        // public Task AddFollowAsync(Follow followCred);
        // public Task<Follow> UpdateAsync(Follow followCred);
        //public Task DeleteFollowAsync(int mentorId, int menteeId, int topicId);

        /// <summary>
        /// Gets info about a specific mentorship between a mentor and a mentee.
        /// </summary>
        /// <param name="menteeId">The id of the mentor.</param>
        /// <param name="mentorId">The id of the mentee.</param>
        /// <param name="topicId">The id of the field of interest the mentor would be providing guidiance on.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains info about a mentorship concerning a mentor and mentee.
        /// </returns>
        public Task<Follow> GetMentorshipInfo(int menteeId, int mentorId, int topicId);

        /// <summary>
        /// Gets IDs of all mentees currently being guided by the mentor.
        /// </summary>
        /// <param name="mentorId">The id of the mentor.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list that contains ID's of all mentees receiving guidiance under mentor.
        /// </returns>
        Task<List<int>> GetMenteesIdAsync(int mentorId);

        /// <summary>
        /// Gets info about all "Pending" and "on Hold" mentorship requests sent to the mentor.
        /// </summary>
        /// <param name="mentorId">The id of the mentor.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list that contains info about all mentorship request sent to the mentor that have not been confirmed.
        /// </returns>
        Task<List<Follow>> GetMentorshipRequestsForMentorAsync(int mentorId);

        /// <summary>
        /// Gets info about all mentorship requests sent by the mentee that have not been confirmed.
        /// </summary>
        /// <param name="menteeId">The id of the mentee.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list that contains info about all mentorship request sent by the mentee that have not been confirmed.
        /// </returns>
        Task<List<Follow>> GetSentMentorshipRequestsForMenteeAsync(int menteeId);

        /// <summary>
        /// Gets info about all ongoing mentorship between a mentor and a mentee.
        /// </summary>
        /// <param name="mentorId">The id of the mentor.</param>
        /// <param name="menteeId">The id of the mentee.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list that contains info about all mentorship between a mentor and mentee.
        /// </returns>
        Task<List<Follow>> GetAllMentorshipInfoBetweenUsersAsync(int mentorId, int menteeId);
    }
}