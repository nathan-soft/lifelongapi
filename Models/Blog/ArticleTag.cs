using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Models
{
    public class ArticleTag
    {
        public int ArticleId { get; set; }
        public int TopicId { get; set; }

        public virtual Article Article { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
