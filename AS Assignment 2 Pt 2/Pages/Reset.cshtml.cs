using AS_Assignment_2_Pt_2.Model;
using AS_Assignment_2_Pt_2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AS_Assignment_2_Pt_2.Pages
{
    public class ResetModel : PageModel
    {

        [BindProperty]
        public Reset ResModel { get; set; }

        private UserManager<Customer> userManager { get; set; }

        public ResetModel(UserManager<Customer> userManager)
        {
            this.userManager = userManager; 
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> ResetPassword()
        {
            if (ModelState.IsValid) 
            { 
                var user = await userManager.FindByEmailAsync(ResModel.Email);
                if (user == null)
                {
                    var result = await userManager.ResetPasswordAsync(user, ResModel.Token, ResModel.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToPage("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return RedirectToPage("Reset");
                }
            }
            return Page();
        }
    }
}
