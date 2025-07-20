using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }

        public string Phone { get; set; }
        public bool UyelikSozlesmesi { get; set; }
        public bool AydinlatmaMetni { get; set; }
    }
}
