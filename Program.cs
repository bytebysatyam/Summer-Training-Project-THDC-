using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebPage.Data;
using WebPage.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["MYSQL_CONNECTION_STRING"]
    ?? throw new InvalidOperationException(
        "A MySQL connection string is required. Set ConnectionStrings__DefaultConnection or MYSQL_CONNECTION_STRING.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();

    var demoAdminEnabled =
        builder.Configuration.GetValue<bool>("DemoAdmin:Enabled") ||
        string.Equals(builder.Configuration["DEMO_ADMIN_ENABLED"], "true", StringComparison.OrdinalIgnoreCase);

    if (demoAdminEnabled)
    {
        var adminEmail = builder.Configuration["DEMO_ADMIN_EMAIL"] ?? "admin@training.local";
        var adminPassword = builder.Configuration["DEMO_ADMIN_PASSWORD"];

        if (string.IsNullOrWhiteSpace(adminPassword))
            throw new InvalidOperationException("DEMO_ADMIN_PASSWORD is required when demo admin seeding is enabled.");

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var normalizedEmail = adminEmail.Trim().ToLowerInvariant();

        if (!await db.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            var admin = new User
            {
                FullName = "Training Admin",
                Email = normalizedEmail,
                Role = "Admin"
            };
            admin.Password = hasher.HashPassword(admin, adminPassword);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

public partial class Program { }
