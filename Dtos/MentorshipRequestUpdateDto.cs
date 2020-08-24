using static LifeLongApi.Codes.AppHelper;

namespace LifeLongApi.Dtos
{
    public class MentorshipRequestUpdateDto
    {
        public string MentorUsername { get; set; }
        public FollowStatus Status { get; set; }
    }
}