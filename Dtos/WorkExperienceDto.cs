using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos
{
    public class WorkExperienceDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string FieldOfInterest { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string EmploymentType { get; set; }
        [Required]
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        [Required]
        public bool CurrentlyWorking { get; set; }
    }
}