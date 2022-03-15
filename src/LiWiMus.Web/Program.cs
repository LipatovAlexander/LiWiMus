using LiWiMus.Core.Entities;
using LiWiMus.Core.Interfaces;
using LiWiMus.Infrastructure.Data;
using LiWiMus.Web.Configuration;
using LiWiMus.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole();

builder.Configuration.AddEnvironmentVariables();

var services = builder.Services;

LiWiMus.Infrastructure.Dependencies.ConfigureServices(builder.Configuration, services);
services.AddCoreServices(builder.Configuration);
services.AddWebServices(builder.Configuration);

services.AddTransient<IRazorViewRenderer, RazorViewRenderer>();

services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 3;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationContext>();
services.AddControllersWithViews();

var app = builder.Build();

app.Logger.LogInformation("\nConnection string: {ConnectionString} \n", builder.Configuration.GetConnectionString("DefaultConnection"));

app.Logger.LogInformation("App created...");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Data")),
    RequestPath = "/Data"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Logger.LogInformation("Seeding Database...");

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    try
    {
        var applicationContext = scopedProvider.GetRequiredService<ApplicationContext>();
        await ApplicationContextSeed.SeedAsync(applicationContext, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Logger.LogInformation("LAUNCHING");

app.Run();