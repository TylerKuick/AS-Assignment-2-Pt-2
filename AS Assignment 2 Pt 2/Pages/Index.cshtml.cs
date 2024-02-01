using AS_Assignment_2_Pt_2.Model;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using System.Text;

namespace AS_Assignment_2_Pt_2.Pages
{
	[Authorize]
	public class IndexModel : PageModel
	{

        private readonly SignInManager<Customer> signInManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        public string MyDBConnection;
        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, SignInManager<Customer> signInManager)
        {
            _logger = logger;
            this.signInManager = signInManager;
            this._configuration = configuration;
            this.MyDBConnection = _configuration.GetConnectionString("AuthConnectionString");
        }

        protected string decryptData(byte[] data)
        {
            string decryptedString = null;
            byte[] IV;
            byte[] Key;
            string target = HttpContext.Session.GetString("LoggedIn");
            SqlConnection connection = new SqlConnection(MyDBConnection);
            string query = "SELECT cryptIV,cryptKey FROM AspNetUsers WHERE Email=@email";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@email", target);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    IV = (byte[])reader.GetValue(0);
                    Key = (byte[])reader.GetValue(1);
                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.IV = IV; // from db
                    cipher.Key = Key;   // from db
                    ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                    byte[] decryptedText = decryptTransform.TransformFinalBlock(data, 0, data.Length);
                    decryptedString = Encoding.UTF8.GetString(decryptedText);
                }
            }
            catch (Exception ex)
            {
                decryptedString = ex.Message;
            }
            finally { connection.Close(); }
            return decryptedString;
        }
        
        public void OnGet()
		{

            var authToken = HttpContext.Session.GetString("AuthToken");
            var email = HttpContext.Session.GetString("LoggedIn");
            if (email != null && authToken != null && Request.Cookies["AuthToken"] != null)
            {
                if (!(authToken.Equals(Request.Cookies["AuthToken"].ToString()))) {
                    signInManager.SignOutAsync();
                    HttpContext.Session.Clear();
                    if (Request.Cookies[".AspNetCore.Session"] != null)
                    {
                        Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions { Expires = DateTime.Now.AddMonths(-20) });
                    }

                    if (Request.Cookies["AuthToken"] != null)
                    {
                        Response.Cookies.Delete("AuthToken", new CookieOptions { Expires = DateTime.Now.AddMonths(-20) });
                    }
                    Response.Redirect("Login", false);
                }
                else
                {
                    TempData["LoggedIn"] = email;
                }
                
            }
            else {
                signInManager.SignOutAsync();
                HttpContext.Session.Clear();
                if (Request.Cookies[".AspNetCore.Session"] != null)
                {
                    Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions { Expires = DateTime.Now.AddMonths(-20) });
                }

                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies.Delete("AuthToken", new CookieOptions { Expires = DateTime.Now.AddMonths(-20) });
                }
                Response.Redirect("Login", false);   
            }

            SqlConnection connection = new SqlConnection(MyDBConnection);
            string query = "SELECT * FROM AspNetUsers WHERE Email=@email";
            SqlCommand  command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@email", email);


            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    
                    var Fname = reader.GetString(1).ToString();
                    var Lname = reader.GetString(2).ToString();
                    var Nric = (byte[])reader.GetValue(4); //Encrypted
                    var Dob = reader.GetString(9);

                    TempData["FirstName"] = Fname;
                    TempData["LastName"] = Lname;
                    TempData["NRIC"] = decryptData(Nric);
                    TempData["DOB"] = Dob;
                }
                
            }
            catch (Exception ex) {
                RedirectToPage("Login");
            }finally { connection.Close(); }

        }

       
    }
}
