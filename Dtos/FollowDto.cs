using System;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos
{
    public class FollowDto
    {
        [Required]
        public string MenteeUsername { get; set; }
        [Required]
        public string MentorUsername { get; set; }
        [Required]
        public string TopicName { get; set; }
        public string Status { get; set; }
    }
}