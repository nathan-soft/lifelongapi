
namespace LifeLongApi.Dtos.Response
{
    public class WorkExperienceResponseDto
    {
        public int Id { get; set; }
        public string FieldOfInterest { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string EmploymentType { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public bool CurrentlyWorking { get; set; }
    }
}