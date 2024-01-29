using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AS_Assignment_2_Pt_2.Pages
{
	[Authorize]
	public class IndexModel : PageModel
	{
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        public string MyDBConnection;
        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            this._configuration = configuration;
            this.MyDBConnection = _configuration.GetConnectionString("AuthConnectionString");
        }

        public void OnGet()
		{
            var authToken = HttpContext.Session.GetString("AuthToken");
            var email = HttpContext.Session.GetString("LoggedIn");
            if (email != null && authToken != null && Request.Cookies["AuthToken"] != null)
            {
                if (!(authToken.Equals(Request.Cookies["AuthToken"].ToString()))) {

                    Response.Redirect("Login", false);
                }
                else
                {
                    _logger.LogInformation(email);
                }
                
            }
            else {
                Response.Redirect("Login", false);
            }
            
        }
	}
}
