using System;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos
{
    public class UserFieldOfInterestDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string TopicName { get; set; }
        [Required]
        public int YearsOfExperience { get; set; }
        [Required]
        public bool CurrentlyWorking { get; set; }
        public bool IsPaid { get; set; }

    }
}