using System.ComponentModel.DataAnnotations;

namespace AS_Assignment_2_Pt_2.ViewModels
{
    public class ChangePassword
    {
        [Required]
        [MinLength(12, ErrorMessage = "Passwords must be at least 12 characters long")]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MinLength(12, ErrorMessage = "Passwords must be at least 12 characters long")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "New Password and Confirmation Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
