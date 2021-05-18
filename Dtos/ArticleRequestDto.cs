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
        [Required, StringLength(70, MinimumLength = 25)]
        public string Title { get; set; }
        [StringLength(70, MinimumLength = 25, ErrorMessage = "The {0} must be between 25 and 70 characters.")]
        public string Excerpt { get; set; }
        [Required]
        public IFormFile UploadedImage { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public List<string> Tags { get; set; }
    }
}
