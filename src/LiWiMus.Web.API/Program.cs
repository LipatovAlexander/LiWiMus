using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using LiWiMus.Infrastructure;
using LiWiMus.Web.Shared.Configuration;
using LiWiMus.Web.Shared.Extensions;
using MinimalApi.Endpoint.Extensions;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host
       .UseServiceProviderFactory(new AutofacServiceProviderFactory())
       .ConfigureContainer<ContainerBuilder>(containerBuilder =>
           containerBuilder.RegisterModule(new ConfigurationCoreModule(builder.Environment.ContentRootPath)));

builder.Configuration.AddSharedSettings(builder.Environment);
Dependencies.ConfigureServices(builder.Configuration, builder.Services);
builder.Services.AddSharedServices();
builder.Services.ConfigureSettings(builder.Configuration);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddIdentity(builder.Environment);
builder.Services.AddFluentValidation(fv =>
{
    fv.LocalizationEnabled = false;
    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddEndpoints();

builder.Services.AddAuthentication(options =>
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services
       .AddOpenIddict()
       .AddValidation(options =>
       {
           options.SetIssuer("https://localhost:5021");
           options.UseSystemNetHttp();
           options.UseAspNetCore();
       });

var app = builder.Build();

app.UseSharedStaticFiles(builder.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();