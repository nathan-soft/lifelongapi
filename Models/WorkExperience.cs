using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Models
{
    public class WorkExperience : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TopicId { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string EmploymentType { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public bool CurrentlyWorking { get; set; }

        public AppUser User { get; set; }
        public Topic Topic { get; set; }
    }
}