using System.ComponentModel.DataAnnotations;

namespace LifeLongApi.Dtos {
    public class RegisterDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [StringLength (100, ErrorMessage = "Please enter at least 10 characters", MinimumLength = 6)]
        [DataType (DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; }
    }
}