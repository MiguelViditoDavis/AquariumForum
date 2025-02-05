using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AquariumForum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure the database context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("AppDbContext")
                    ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")
                ));

            // Add services to the container
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Enable HTTP Strict Transport Security
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Enable serving static files (needed for CSS, JS, images, etc.)
            app.UseRouting();
            app.UseAuthorization();

            // Configure default route mapping
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
