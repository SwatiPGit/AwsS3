using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Diagnostics;

namespace ConsumeAwsS3Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        else 
        {
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            app.UseExceptionHandler("/Home/Exception");

        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}