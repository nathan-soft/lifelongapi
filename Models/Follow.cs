using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeLongApi.Models {
    public class Follow : BaseEntity {
        public int Id { get; set; }


        [ForeignKey("Mentee")]
        public int MenteeId { get; set; }
        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        public int TopicId { get; set; }
        
        [Required]
        public string Status { get; set; }

        public virtual AppUser Mentor { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual AppUser Mentee { get; set; }
    }
}