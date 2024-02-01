using AS_Assignment_2_Pt_2.Model;
using AS_Assignment_2_Pt_2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AS_Assignment_2_Pt_2.Pages
{
    [Authorize]
    public class VerifyModel : PageModel
    {
        [BindProperty]
        public Verify VModel { get; set; }
        private readonly UserManager<Customer> userManager; 
        private readonly SignInManager<Customer> signInManager;
        private readonly IConfiguration _configuration;

        public VerifyModel(UserManager<Customer> userManager, SignInManager<Customer> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._configuration = configuration;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync() {
            var user = await userManager.FindByEmailAsync(VModel.Email);
            
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResetLink = Url.Action("OnPostAsync", "Verify",
                    new { email = VModel.Email, token }, Request.Scheme);
                TempData["ErrorMsg"] = passwordResetLink;
                return RedirectToPage("token");
            }
            TempData["ErrorMsg"] = "null";
            
            return Page();
        }
    }
}
