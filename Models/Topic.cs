using System.Collections.Generic;

namespace LifeLongApi.Models {
    public class Topic : BaseEntity {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public Category Category { get; set; }
        public IList<Follow> Follows { get; set; }
        public IList<UserFieldOfInterest> UserFieldOfInterests { get; set; }
    }
}