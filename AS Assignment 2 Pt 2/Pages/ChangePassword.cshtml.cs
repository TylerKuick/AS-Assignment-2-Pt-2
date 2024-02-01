using AS_Assignment_2_Pt_2.Model;
using AS_Assignment_2_Pt_2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace AS_Assignment_2_Pt_2.Pages
{
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public ChangePassword ChangePassModel { get; set; }

        private readonly SignInManager<Customer> signInManager;
        private readonly UserManager<Customer> userManager;
        private readonly IConfiguration _configuration;
        public string MyDBConnection;

        public ChangePasswordModel(SignInManager<Customer> signInManager, UserManager<Customer> userManager)
        {
            this.signInManager = signInManager; 
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid) {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMsg"] = "user null";
                    return RedirectToPage("Login");
                }
                TempData["ErrorMsg"] = user.Password;
                var result = await userManager.ChangePasswordAsync(user, ChangePassModel.Password, ChangePassModel.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return Page();
                }

                await signInManager.RefreshSignInAsync(user);
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
