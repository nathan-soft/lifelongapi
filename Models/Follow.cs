using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Models {
    public class Follow : BaseEntity {
        public AppUser User { get; set; }
        public int UserId { get; set; }
        public AppUser Follower { get; set; }
        public int FollowingMentorId { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }
        [Required]
        public string Status { get; set; }
    }
}