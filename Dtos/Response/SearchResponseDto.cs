using System.Collections.Generic;

namespace LifeLongApi.Dtos.Response
{
    public class SearchResponseDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }


        public int MenteesCount { get; set; }
        public List<UserFieldOfInterestDto> UserFieldOfInterests { get; set; }
    }
}