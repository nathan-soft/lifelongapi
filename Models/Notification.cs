using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeLongApi.Models
{
    public class Notification : BaseEntity
    {
        public int Id { get; set; }
        public int CreatedById { get; set; }
        public int CreatedForId { get; set; }
        [Required]
        public string Message { get; set; }
        public bool IsSeen { get; set; }
        [Required]
        public string Type { get; set; }

        [ForeignKey("CreatedById")]
        public virtual AppUser CreatedBy { get; set; }
        [ForeignKey("CreatedForId")]
        public virtual AppUser CreatedFor { get; set; }
    }
}