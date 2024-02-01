using System.ComponentModel.DataAnnotations;

namespace AS_Assignment_2_Pt_2.ViewModels
{
    public class Reset
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MinLength(12, ErrorMessage = "Passwords must be at least 12 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
