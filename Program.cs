using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity + Roles (CORRECTION CS0411 : Ajout explicite des types)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();


// =========================
// INITIALISATION / SEEDING
// =========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Migration automatique avant le seeding
    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    await SeedRolesAsync(services);
    await SeedAdminUserAsync(services);
}


// Middleware / pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();


// =========================
// METHODS
// =========================
static async Task SeedRolesAsync(IServiceProvider services)
{
    // CORRECTION : Spécification du type RoleManager
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

static async Task SeedAdminUserAsync(IServiceProvider services)
{
    // CORRECTION : Spécification des types UserManager et RoleManager
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string adminUserName = "admin";
    string adminEmail = "admin@forum.com";
    string adminPassword = "Admin123!";

    var user = await userManager.FindByEmailAsync(adminEmail);

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, adminPassword);

        if (!result.Succeeded)
        {
            throw new Exception("Impossible de créer l'utilisateur admin.");
        }
    }
    else
    {
        // Si l'utilisateur existe déjà, réinitialiser son mot de passe
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        await userManager.ResetPasswordAsync(user, token, adminPassword);
    }

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await userManager.IsInRoleAsync(user, "Admin"))
    {
        await userManager.AddToRoleAsync(user, "Admin");
    }
}
