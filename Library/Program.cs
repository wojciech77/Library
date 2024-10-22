using System;
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

var authenticationSettings = new AuthenticationSettings();

// Read values from environment variables or fallback to appsettings
authenticationSettings.JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Authentication:JwtIssuer"];
authenticationSettings.JwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Authentication:JwtKey"];

// Add services to the container.
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddCookie(x =>
{
    x.Cookie.Name = "token";
})
.AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
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

builder.Services.AddDbContext<LibraryContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("LibraryConnectionString"))
);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

var app = builder.Build();

// Seeding the admin user during application startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    // Seed admin account only if it doesn't exist
    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "default_admin@gmail.com";
    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Password123";

    if (!context.Users.Any(u => u.Email == adminEmail))
    {
        var adminUser = new User
        {
            Id = Guid.NewGuid(),  // Generate new GUID
            Email = adminEmail,
            FirstName = Environment.GetEnvironmentVariable("ADMIN_FIRST_NAME") ?? "Admin",
            LastName = Environment.GetEnvironmentVariable("ADMIN_LAST_NAME") ?? "Nimda",
            RoleId = int.TryParse(Environment.GetEnvironmentVariable("ADMIN_ROLE_ID"), out var roleId) ? roleId : 3
        };

        var hashedPassword = passwordHasher.HashPassword(adminUser, adminPassword);
        adminUser.PasswordHash = hashedPassword;

        context.Users.Add(adminUser);
        context.SaveChanges();  // Save the new admin user to the database
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
