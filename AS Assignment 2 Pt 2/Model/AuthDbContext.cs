using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AS_Assignment_2_Pt_2.Model
{
	public class AuthDbContext : IdentityDbContext<Customer>
	{ 
		private readonly IConfiguration _configuration;	
		public AuthDbContext(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string connectionString = _configuration.GetConnectionString("AuthConnectionString");
			optionsBuilder.UseSqlServer(connectionString);

		}
	}
}
