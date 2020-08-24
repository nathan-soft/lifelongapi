using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos.Response
{
    public class QualificationResponseDto
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string QualificationType { get; set; }
        public string Major { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }
}