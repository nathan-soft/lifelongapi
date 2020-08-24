using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Models
{
	public class UserFieldOfInterest : BaseEntity
	{
        public int UserId { get; set; }
        public virtual AppUser User { get; set; }
        
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }
        [Required]
        public int YearsOfExperience { get; set; }
        public bool CurrentlyWorking { get; set; }
        public bool IsPaid { get; set; }
    }
}
