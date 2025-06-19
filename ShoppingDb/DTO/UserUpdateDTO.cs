using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.DTO
{
    public class UserUpdateDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }


    }
}
