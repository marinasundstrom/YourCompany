using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.IdentityManagement.Client;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;

namespace YourBrand.IdentityManagement;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddIdentityManagementClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"https://acme.yourbrand.local:5174/api/identityservice/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("administration", () => resources["Administration"]) ?? navManager.CreateGroup("administration", () => resources["Administration"]);
        group.RequiresAuthorization = true;
        //group.Roles = new[] { "Foo" } ;

        group.CreateItem("users", () => resources["Users"], MudBlazor.Icons.Material.Filled.People, "/usermanagement/users");
        group.CreateItem("organization", () => resources["Organization"], MudBlazor.Icons.Material.Filled.House, "/usermanagement/organization");

        group.CreateItem("sync-data", () => resources["Sync data"], MudBlazor.Icons.Material.Filled.Sync, "/sync-data");
    }
}