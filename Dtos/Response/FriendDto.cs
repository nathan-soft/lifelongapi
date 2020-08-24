using System.Collections.Generic;

namespace LifeLongApi.Dtos.Response
{
    public class FriendDto
    {
        private string location;

        public AbbrvUser User { get; set; }
        public string Location
        {
            get => location; 
            set{
                //if the user hasn't specified location yet.
                location = value != ".." ? value : null;
            }
        }

        public List<MutualInterestDto> MutualInterests { get; set; }
    }

    public class MutualInterestDto{
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MentorFriendDto : FriendDto{}
    public class MenteeFriendDto : FriendDto { }
}