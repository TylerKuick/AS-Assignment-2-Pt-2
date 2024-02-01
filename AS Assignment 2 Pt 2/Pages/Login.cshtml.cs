using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AS_Assignment_2_Pt_2.ViewModels;
using Microsoft.AspNetCore.Identity;
using AS_Assignment_2_Pt_2.Model;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using System;
using Nancy.Json;
using System.Net;
using System.Web;
using System.Text.Json;


namespace AS_Assignment_2_Pt_2.Pages
{
    [AutoValidateAntiforgeryToken]
    public class LoginModel : PageModel
    {
        

        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<Customer> signInManager;
        private readonly UserManager<Customer> userManager;
        private readonly IConfiguration _configuration;
        public string MyDBConnection;
        //string MyDBConnection = System.Configuration.ConfigurationManager.ConnectionStrings["AuthConnectionString"].ConnectionString.ToString();
  
        public LoginModel(SignInManager<Customer> signInManager, IConfiguration configuration) { 
            this.signInManager = signInManager;   
            this._configuration = configuration;
            this.MyDBConnection = _configuration.GetConnectionString("AuthConnectionString");
        }

       
        public void OnGet()
        {
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MyDBConnection);
            string sql = "select Password FROM AspNetUsers WHERE Email=@UserName";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserName", userid);
            try {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) { 
                        if (reader["Password"] != null)
                        {
                            if (reader["Password"] != DBNull.Value)
                            {
                                h = reader["Password"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;

        }

        protected string getDBSalt(string userid) {
            string h = null;
            SqlConnection connection = new SqlConnection(MyDBConnection);
            string sql = "select PasswordSalt FROM AspNetUsers WHERE Email=@UserName";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserName", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                h = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;

        }

        public class MyObject
        {
            public bool success { get; set; }
            public DateTime? challenge_ts {  get; set; }
            public string? hostname {  get; set; }
            public float? score {  get; set; }
            public string? action { get; set; }
            public List<string>? ErrorMessage { get; set; }
        }
        public Boolean ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
                $"https://www.google.com/recaptcha/api/siteverify?secret=6LesYmApAAAAAN7qQJ9s9JpKpuVV3KRVdt1POHlO&response={captchaResponse}");

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        MyObject jsonObject = JsonSerializer.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (ModelState.IsValid)
            {
                string pwd = LModel.Password;
                string email = LModel.Email.Trim();

                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);

                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);

                    if (ValidateCaptcha())
                    {
                        var identityResult = await signInManager.PasswordSignInAsync(HttpUtility.HtmlEncode(LModel.Email), HttpUtility.HtmlEncode(LModel.Password), LModel.RememberMe, true);

                        if (userHash == dbHash)
                        {
                            if (identityResult.IsLockedOut)
                            {
                                TempData["ErrorMsg"] = "Account Locked Out. Try again in 15 seconds.";
                                return RedirectToPage("Login");
                            }
                            HttpContext.Session.SetString("LoggedIn", LModel.Email);
                            string guid = Guid.NewGuid().ToString();
                            HttpContext.Session.SetString("AuthToken", guid);
                            Response.Cookies.Append("AuthToken", guid);
                            return RedirectToPage("Index"); 
                        }
                        else
                        {
                            //var userExist = await userManager.FindByLoginAsync(HttpUtility.HtmlEncode(LModel.Email), HttpUtility.HtmlEncode(LModel.Password));  
                            //var result = signInManager.Find
                            await signInManager.SignOutAsync();
                            TempData["ErrorMsg"] = "Username or Password Incorrect";
                            if (identityResult.IsLockedOut)
                            {
                                TempData["ErrorMsg"] = "Account Locked Out. Try again in 15 seconds.";
                                return RedirectToPage("Login");
                            }
                            return RedirectToPage("Login");
                        }
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Captcha Failed";
                        return RedirectToPage("Login");
                    }
                }
            }

            return Page();
        }
    }
}
