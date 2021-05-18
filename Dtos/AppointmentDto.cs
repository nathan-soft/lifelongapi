using System;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos
{
    public class AppointmentDto
    {
        [Required]
        public string MenteeUsername { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Date { get; set; }
        public string Time { get; set; }
    }
}