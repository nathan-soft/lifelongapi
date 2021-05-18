using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Dtos
{
    public class MailDto
    {
        [Required, EmailAddress]
        public string UserEmail { get; set; }
        [Required, StringLength(30, MinimumLength = 2, ErrorMessage = "The {0} must be between 2 and 30 characters.")]
        public string Subject { get; set; }
        [Required, StringLength(5000, MinimumLength = 2, ErrorMessage = "The {0} must be between 2 and 5000 characters.")]
        public string Message { get; set; }
    }
}
