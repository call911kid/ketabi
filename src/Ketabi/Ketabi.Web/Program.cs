using System.Text;
using Ketabi.Application;
using Ketabi.Application.Common;
using Ketabi.Application.Interfaces;
using Ketabi.Infrastructure;
using Ketabi.Infrastructure.Authentication;
using Ketabi.Web.Mappings;
using Ketabi.Web.Middlewares;
using Ketabi.Web.Realtime;
using Ketabi.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Ketabi.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Add application services
            builder.Services.AddApplicationServices();

            // Add infrastructure services
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // Add mapping services
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AccountWebProfile>();
            }, typeof(AccountWebProfile).Assembly);

            builder.Services.AddScoped<IFileService, FileService>();

            // Add controllers with views
            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR();
            builder.Services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"];

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Login";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey!)),

                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue(AppConstants.AuthCookieName, out var token))
                            {
                                context.Token = token;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });


            var app = builder.Build();

            using (var scope = app.Services.CreateScope()) // seeding roles
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                foreach (var role in new[] { RolesConstants.User, RolesConstants.Admin, RolesConstants.Moderator })
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));

                        if (!result.Succeeded)
                            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

            }


            // Configure the HTTP request pipeline.
            app.UseCustomExceptionHandler();

            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseUnderDevelopmentGuard();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/hubs/notifications");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
