using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LifeLongApi.Models {
    public class AppUser : IdentityUser<int> {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        // public int MenteesCount { get; set; }
        
        public List<Qualification> Qualifications { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }
        public virtual List<Follow> Following { get; set; }
        public virtual List<Follow> Followers { get; set; }
        public List<AppRole> Roles { get; set; }
        public List<UserFieldOfInterest> UserFieldOfInterests { get; set; }
    }
}