using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Dtos
{
    public class ArticleRequestDto
    {
        [Required, StringLength(70)]
        public string Title { get; set; }
        [Required]
        public IFormFile UploadedImage { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public List<string> Tags { get; set; }
    }
}
