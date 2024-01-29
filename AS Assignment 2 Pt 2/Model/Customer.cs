using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AS_Assignment_2_Pt_2.Model
{
    public class Customer : IdentityUser    
    {
        [Required]
        [DataType(DataType.Text)]
        public string Fname { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Lname { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [MinLength(12, ErrorMessage = "Passwords must be at least 12 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        public string PasswordSalt { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public string DOB { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public string Resume { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string WhoAmI { get; set; }
    }
}
