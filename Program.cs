using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AvukatSistemi.Data;
using AvukatSistemi.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity servisi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Roller ve admin kullanıcı oluşturuluyor
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRolesAndAdmin(userManager, roleManager);
}

// Middleware ve yönlendirme
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// 🔧 Admin ve Client rolünü oluştur + admin kullanıcı
async Task SeedRolesAndAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    string adminEmail = "admin@site.com";
    string adminPassword = "Admin123!";

    string[] roles = new[] { "Admin", "Client" };

    // 🔹 Rolleri oluştur (sadece eksik olanları)
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 🔹 Admin kullanıcıyı oluştur
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Admin kullanıcı oluşturulamadı: {error.Description}");
            }
        }
    }
    else
    {
        // 🔹 Admin rolünde değilse ekle
        var rolesForUser = await userManager.GetRolesAsync(adminUser);
        if (!rolesForUser.Contains("Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
