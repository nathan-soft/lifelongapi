using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeLongApi.Models {
    public class AppUser : IdentityUser<int> {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        [Required]
        public string TimeZone { get; set; }

        // public int MenteesCount { get; set; }

        public virtual List<Article> Articles { get; set; }

        [InverseProperty("Mentor")]
        public virtual List<Appointment> MentorAppointments { get; set; }
        [InverseProperty("Mentee")]
        public virtual List<Appointment> MenteeAppointments { get; set; }
        [InverseProperty("CreatedBy")]
        public virtual List<Notification> CreatedByNotifications { get; set; }
        [InverseProperty("CreatedFor")]
        public virtual List<Notification> CreatedForNotifications { get; set; }
        public virtual List<Qualification> Qualifications { get; set; }
        public virtual List<WorkExperience> WorkExperiences { get; set; }
        [InverseProperty("Mentor")]
        public virtual List<Follow> Mentors { get; set; }
        [InverseProperty("Mentee")]
        public virtual List<Follow> Mentees { get; set; }
        //public virtual List<AppRole> Roles { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual List<UserFieldOfInterest> UserFieldOfInterests { get; set; }
    }

    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public virtual AppUser User { get; set; }
        public virtual AppRole Role { get; set; }
    }
}