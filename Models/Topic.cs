using System.Collections.Generic;

namespace LifeLongApi.Models {
    public class Topic : BaseEntity {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual Category Category { get; set; }
        public virtual List<Follow> Follows { get; set; }
        public virtual List<UserFieldOfInterest> UserFieldOfInterests { get; set; }
    }
}