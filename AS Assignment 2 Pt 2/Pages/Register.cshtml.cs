using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AS_Assignment_2_Pt_2.ViewModels;
using System.Text.RegularExpressions;
using AS_Assignment_2_Pt_2.Model;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using System.Web;

namespace AS_Assignment_2_Pt_2.Pages
{
    public class RegisterModel : PageModel
    {
        public byte[] IV;
        public byte[] Key;
        private UserManager<Customer> userManager { get; }
        private SignInManager<Customer> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }
        

        public RegisterModel(UserManager<Customer> userManager, 
            SignInManager<Customer> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public void OnGet()
        {
            
        }

        protected byte[] encryptData(string data)
        {
            string encryptedText;
            byte[] cipherText = null;
            try {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            } 
            catch (Exception ex) {
                throw new Exception(ex.ToString());
            }
            return cipherText;
        }

		public async Task<IActionResult> OnPostAsync()
		{
            if (ModelState.IsValid)
            {
                string pwd = RModel.Password;
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                rng.GetBytes(saltByte);
                var salt = Convert.ToBase64String(saltByte);    
                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = pwd + salt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                var finalHash = Convert.ToBase64String(hashWithSalt);

                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                var user = new Customer()
                {
                    UserName = RModel.Email,
                    Fname = HttpUtility.HtmlEncode(RModel.Fname.Trim()),
                    Lname = HttpUtility.HtmlEncode(RModel.Lname.Trim()),
                    Gender = HttpUtility.HtmlEncode(RModel.Gender.Trim()),
                    NRIC = encryptData(HttpUtility.HtmlEncode(RModel.NRIC.Trim())),
                    Email = HttpUtility.HtmlEncode(RModel.Email),
                    Password = finalHash,
                    ConfirmPassword = finalHash,
                    PasswordSalt = salt,
                    DOB = RModel.DOB,
                    Resume = RModel.Resume,
                    WhoAmI = HttpUtility.HtmlEncode(RModel.WhoAmI.Trim()),
                    cryptIV = IV,
                    cryptKey = Key

                };
                
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("Login");

                } 

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
		}
	}
}
