using GreenfieldLocalHubWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Retrieve database connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Register DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// Enables detailed error pages for database migrations in development
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity with role support and account confirmation requirement
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add support for roles
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Register MVC controllers with views
builder.Services.AddControllersWithViews();

// Configure Google OAuth authentication for external logins
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
});

var app = builder.Build();

// Seed initial data into database on application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Populate roles, admin user, producers, categories and products
    await SeedData.seedRolesAndUsers(services, userManager, roleManager);
    await SeedData.seedProducers(services);
    await SeedData.seedCategories(services);
    await SeedData.seedProducts(services);
    await SeedData.seedAddresses(services);
    await SeedData.seedOrders(services);
    await SeedData.seedLoyaltyAccounts(services);
}

// Configure HTTP request pipeline middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Provides DB migration error pages in development
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Custom error page for production
    app.UseHsts(); // Enforce HTTPS with HSTS (30 day default)
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();
// Enable routing middleware
app.UseRouting();

// Enable authorization middleware (must come after UseRouting)
app.UseAuthorization();

// Serve static files (CSS, JS, images) with caching headers
app.MapStaticAssets();

// Default MVC route: Controller=Home, Action=Index, id is optional
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map Razor Pages (used by Identity UI)
app.MapRazorPages()
   .WithStaticAssets();

// Start the application
app.Run();