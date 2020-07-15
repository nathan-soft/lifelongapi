using System;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos.Response
{
    public class FollowResponseDto
    {
        [Display(Name = "Mentee Id")]
        public int UserId { get; set; }

        [Display(Name = "Mentee Username")]
        public string MenteeUsername { get; set; }

        [Display(Name = "Mentee Fullname")]
        public string MenteeFullname { get; set; }

        [Display(Name = "Mentor id")]
        public int FollowingMentorId { get; set; }

        [Display(Name = "Mentor username")]
        public string FollowingMentorUsername { get; set; }

        [Display(Name = "Field of interest id")]
        public int TopicId { get; set; }

        [Display(Name = "Field of interest name")]
        public string TopicName { get; set; }

        public string Status { get; set; }
    }
}