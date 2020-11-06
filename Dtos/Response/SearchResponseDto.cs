using System.Collections.Generic;

namespace LifeLongApi.Dtos.Response
{
    public class SearchResponseDto
    {
        private string location;
        
        public AbbrvUser User { get; set; }
        public string Location {
            get => location;
            set
            {
                //if the user hasn't specified location yet.
                location = value != ".." ? value : null;
            }
         }

        public int MenteesCount { get; set; }
        public List<UserFieldOfInterestDto> UserFieldOfInterests { get; set; }
    }
}