using Microsoft.EntityFrameworkCore;
using mobilestore.Data;
using System;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using mobilestore.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.DataProtection;

namespace mobilestore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddDataProtection()
                .SetApplicationName("mobilestore")
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "Keys")));
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("ru-RU");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("ru-RU") };
                options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("ru-RU") };
            });

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
            });
            builder.Services.AddScoped<CartController>();
            builder.Services.AddScoped<WishlistController>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.Cookie.Name = ".mobilestore.Auth";
                    options.LoginPath = "/Home/Login";
                    options.LogoutPath = "/Home/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                });
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    await next();
                    return;
                }

                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    using var scope = app.Services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    var cartId = $"user_{userId}";
                    var cartItemsCount = await dbContext.CartItems
                        .Where(ci => ci.CartId == cartId)
                        .SumAsync(ci => ci.Quantity);
                    
                    var user = await dbContext.Users.FindAsync(int.Parse(userId));
                    
                    if (context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>() != null)
                    {
                        context.Items["CartItemsCount"] = cartItemsCount;
                        context.Items["UserBalance"] = user?.Balance ?? 0;
                    }
                }

                await next();
            });

            app.MapControllerRoute(
                name: "profile",
                pattern: "Profile",
                defaults: new { controller = "Account", action = "Profile" });

            app.MapControllerRoute(
                name: "wishlist",
                pattern: "Wishlist",
                defaults: new { controller = "Wishlist", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "admin",
                pattern: "admin/{action=Index}/{id?}",
                defaults: new { controller = "Admin" });

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try 
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.EnsureCreated();
                    
                    if (!context.Products.Any())
                    {
                        DbInitializer.Initialize(context);
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Произошла ошибка при инициализации базы данных.");
                    throw;
                }
            }

            app.Run();
        }
    }
}
