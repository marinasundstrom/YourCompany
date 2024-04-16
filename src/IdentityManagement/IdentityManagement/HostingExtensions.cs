using Duende.IdentityServer;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;

using YourBrand.Extensions;
using YourBrand.IdentityManagement.Domain.Entities;
using YourBrand.IdentityManagement.Infrastructure.Persistence;

namespace YourBrand.IdentityManagement;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<User>()
            //.AddCorsPolicyService<DefaultCorsPolicyService>()
            .AddExtensionGrantValidator<TokenExchangeGrantValidator>()
            .AddProfileService<CustomProfileService<User>>();

        builder.Services.AddAuthorization();

        builder.Services.AddAuthenticationServices(builder.Configuration);

        /*
        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });
            */

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapObservability();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApiAndSwaggerUi();
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors("MyPolicy");

        app.UseAuthentication();
        
        app.UseAuthorization();

        app.UseIdentityServer();

        app.MapControllers();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}