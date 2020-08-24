using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeLongApi.Models
{
    public class Notification : BaseEntity
    {
        public int Id { get; set; }
        [ForeignKey("CreatedBy")]
        public int CreatedById { get; set; }
        [ForeignKey("CreatedFor")]
        public int CreatedForId { get; set; }
        [Required]
        public string Message { get; set; }
        public bool IsSeen { get; set; }
        [Required]
        public string Type { get; set; }

        public virtual AppUser CreatedBy { get; set; }
        public virtual AppUser CreatedFor { get; set; }
    }
}