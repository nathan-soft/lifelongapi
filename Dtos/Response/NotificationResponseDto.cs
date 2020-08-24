using System;
using Humanizer;

namespace LifeLongApi.Dtos.Response
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public AbbrvUser Creator { get; set; }
        public string Message { get; set; }
        public bool IsSeen { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn {
            get {return CreatedOn;}
            set {
                TimeZoneInfo
                            .ConvertTime(value,
                                         TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"))
                            .Humanize();
                //value.ToLocalTime().Humanize()
            }
        }
        
    }
}