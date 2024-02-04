using AS_Assignment_2_Pt_2.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<Customer, IdentityRole>(identityOptions =>
{
	identityOptions.Lockout.MaxFailedAccessAttempts = 3;
	identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(15);	
	identityOptions.Password.RequireNonAlphanumeric = true;
	identityOptions.Password.RequiredLength = 12;
	identityOptions.Password.RequireDigit = true;
	identityOptions.Password.RequireLowercase = true;
	identityOptions.Password.RequireUppercase = true;

}).AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();	
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.ConfigureApplicationCookie(Config =>
{
	Config.LoginPath = "/Login";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/CustomError");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
