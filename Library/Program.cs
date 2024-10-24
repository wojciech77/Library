using FluentValidation;
using FluentValidation.AspNetCore;
using Library.Data;
using Library.Models;
using Library.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load authentication settings from environment variables or appsettings
var authenticationSettings = new AuthenticationSettings
{
    JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Authentication:JwtIssuer"],
    JwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Authentication:JwtKey"],
    JwtExpireDays = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIREDAYS"), out var expireDays)
                    ? expireDays
                    : int.Parse(builder.Configuration["Authentication:JwtExpireDays"])
};

builder.Services.AddSingleton(authenticationSettings);

// Configure authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
    cfg.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Configure database context
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryConnectionString"))
);

// Add services for password hashing, FluentValidation, etc.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

var app = builder.Build();

// Seed admin user if not exists
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "default_admin@gmail.com";
    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Password123";

    if (!context.Users.Any(u => u.Email == adminEmail))
    {
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = adminEmail,
            FirstName = Environment.GetEnvironmentVariable("ADMIN_FIRST_NAME") ?? "Admin",
            LastName = Environment.GetEnvironmentVariable("ADMIN_LAST_NAME") ?? "Admin",
            RoleId = int.TryParse(Environment.GetEnvironmentVariable("ADMIN_ROLE_ID"), out var roleId) ? roleId : 3
        };

        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);

        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
