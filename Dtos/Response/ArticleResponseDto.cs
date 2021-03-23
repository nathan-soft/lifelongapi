using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Dtos.Response
{
    public class ArticleResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Body { get; set; }

        public AbbrvUser Author { get; set; }
        public virtual List<string> ArticleTags { get; set; }
    }
}
