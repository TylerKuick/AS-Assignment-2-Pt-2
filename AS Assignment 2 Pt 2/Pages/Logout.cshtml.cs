using AS_Assignment_2_Pt_2.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net;
using System.IO;
using Nancy.Json;

namespace AS_Assignment_2_Pt_2.Pages
{
    public class LogoutModel : PageModel
    {
        

        private readonly SignInManager<Customer> signInManager;

        public LogoutModel(SignInManager<Customer> signInManager)
        {
            this.signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            if (Request.Cookies[".AspNetCore.Session"] != null)
            {
                Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions { Expires = DateTime.Now.AddMonths(-20)});
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies.Delete("AuthToken", new CookieOptions { Expires = DateTime.Now.AddMonths(-20) });
            }
            
            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
