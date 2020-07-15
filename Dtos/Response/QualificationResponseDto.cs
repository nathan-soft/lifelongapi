using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos.Response
{
    public class QualificationResponseDto
    {
        public int Id { get; set; }
        [Required]
        public string SchoolName { get; set; }
        [Required]
        public string QualificationType { get; set; }
        [Required]
        public string Major { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }
}