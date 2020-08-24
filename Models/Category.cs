using System.Collections.Generic;

namespace LifeLongApi.Models {
    public class Category  : BaseEntity{
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Topic> Topics { get; set; }
    }
}