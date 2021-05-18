using System;
using System.Collections.Generic;
using LifeLongApi.Dtos.Response;

namespace LifeLongApi.Dtos
{
    public class UserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public List<FriendDto> Friends { get; set; }
        public List<QualificationResponseDto> Qualifications { get; set; }
        public List<UserFieldOfInterestDto> UserFieldOfInterests { get; set; }
        public List<WorkExperienceResponseDto> WorkExperiences { get; set; }
        
    }

    public class AppUserResponseDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public List<string> Roles { get; set; }
    }
}