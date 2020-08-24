using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeLongApi.Models
{
    public class Appointment : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        [ForeignKey("Mentee")]
        public int MenteeId { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string Status { get; set; }
        public DateTime DateAndTime { get; set; }

        public virtual AppUser Mentor { get; set; }
        public virtual AppUser Mentee { get; set; }
        public virtual Reminder Reminder { get; set; }
    }

    public class Reminder{
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}