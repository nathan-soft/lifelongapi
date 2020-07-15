using System.Collections.Generic;

namespace LifeLongApi.Models {
    public class Category  : BaseEntity{
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Topic> Topics { get; set; }
    }
}