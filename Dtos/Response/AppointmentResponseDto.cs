using System;

namespace LifeLongApi.Dtos.Response
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public AbbrvUser Mentee { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DateAndTime { get; set; }
    }

    public class AbbrvUser
    {
        public string FullName { get; set; }
        public string Username { get; set; }
    }
}