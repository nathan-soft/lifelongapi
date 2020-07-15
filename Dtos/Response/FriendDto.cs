using System.Collections.Generic;

namespace LifeLongApi.Dtos.Response
{
    public class FriendDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Location { get; set; }

        public List<MutualInterestDto> MutualInterests { get; set; }
    }

    public class MutualInterestDto{
        public int Id { get; set; }
        public string Name { get; set; }
    }
}