using System.Collections.Generic;

namespace LifeLongApi.Models {
    public class SubCategory  : BaseEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

    }
}