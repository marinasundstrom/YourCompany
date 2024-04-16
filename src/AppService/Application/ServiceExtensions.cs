using System.Net.Http.Headers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Identity;
using YourBrand.Notifications.Client;

namespace YourBrand.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));

        services.AddScoped<Handler>();

        services.AddNotificationsClients((sp, http) =>
        {
            http.BaseAddress = new Uri("https://localhost:5030/");
        },
        builder =>
        {
            builder.AddHttpMessageHandler<Handler>();
        });

        return services;
    }
}

public class Handler(IUserContext userContext) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", userContext.GetAccessToken());

        return base.SendAsync(request, cancellationToken);
    }
}