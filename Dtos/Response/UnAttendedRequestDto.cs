using System.Runtime.CompilerServices;
using System;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos.Response
{
    public class UnAttendedRequestDto
    {
        public int Id {get; set;}

        [Display(Name = "FieldOfInterestId")]
        public int TopicId { get; set; }

        [Display(Name = "FieldOfInterestName")]
        public string TopicName { get; set; }

        public string Status { get; set; }

        public AbbrvUser User { get; set; }
    }

    public class UnAttendedRequestForMenteeDto : UnAttendedRequestDto
    {
    }

    public class UnAttendedRequestForMentorDto : UnAttendedRequestDto
    {
    }
}