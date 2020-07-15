using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos
{
    public class TopicDto
    {
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}