using System.ComponentModel.DataAnnotations;

namespace AS_Assignment_2_Pt_2.ViewModels
{
    public class Verify
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
