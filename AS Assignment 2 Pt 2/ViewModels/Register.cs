using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace AS_Assignment_2_Pt_2.ViewModels
{
	public class Register
	{
		[Required]
		[Display(Name ="First Name")]
		[DataType(DataType.Text)]
		public string Fname { get; set; }

		[Required]
		[Display(Name ="Last Name")]
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
		public string Email { get; set; }

		[Required]
		[MinLength(12, ErrorMessage = "Passwords must be at least 12 characters long")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		
		[Required]
		[DataType(DataType.Password)]
		[Display(Name ="Confirm Password")]
		[Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
		public string ConfirmPassword { get; set;}

		
		[Required]
		[Display(Name ="Date of Birth")]
		[DataType(DataType.Date)]
		public string DOB { get; set; }

		[Required]
		[DataType(DataType.Upload)]
		[RegularExpression(@"(.*?)\.(docx|pdf|DOCX|PDF)")]
		public string Resume { get; set; }

		[Required]
		[Display(Name ="Who Am I")]
		[DataType(DataType.MultilineText)]
		public string WhoAmI { get; set; }
    }
}
