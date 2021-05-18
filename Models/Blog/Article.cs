using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Models
{
    public class Article : BaseEntity
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        [MaxLength(70)]
        [Required]
        public string Title { get; set; }
        [MaxLength(70), MinLength(25)]
        public string Excerpt { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string Body { get; set; }

        public virtual List<ArticleTag> ArticleTags { get; set; }
        public virtual AppUser Author { get; set; }
    }
}
