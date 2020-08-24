using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Models
{
    public class Qualification : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string SchoolName { get; set; }
        [Required]
        public string QualificationType { get; set; }
        [Required]
        public string Major { get; set; }
        [Required]
        public int StartYear { get; set; }
        [Required]
        public int EndYear { get; set; }

        public virtual AppUser User { get; set; }
    }
}