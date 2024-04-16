using Hangfire;
using Hangfire.SqlServer;

using MassTransit;

using Microsoft.Data.SqlClient;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Notifications;
using YourBrand.Notifications.Application;
using YourBrand.Notifications.Infrastructure;
using YourBrand.Notifications.Infrastructure.Persistence;
using YourBrand.Tenancy;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Notifications";
string ServiceVersion = "1.0";

// Add services to container

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All)
    .AddApiVersioningServices();

builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

builder.Services.AddProblemDetails();

var configuration = builder.Configuration;

var services = builder.Services;

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .WithOrigins("https://yourbrand.local:5174", "https://*.yourbrand.local:5174")
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                            .Build();
                      });
});


services.AddApplication(configuration);
services.AddInfrastructure(configuration);
services.AddServices();

builder.Services
    .AddUserContext()
    .AddTenantContext();

services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddHttpContextAccessor();

services.AddEndpointsApiExplorer();

services.AddAuthorization();

services.AddAuthenticationServices(configuration);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});

// Add Hangfire services.
builder.Services.AddHangfire(settings => settings
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

var app = builder.Build();

//app.UseSerilogRequestLogging();

app.MapObservability();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApiAndSwaggerUi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapHangfireDashboard();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

var configuration2 = app.Services.GetRequiredService<IConfiguration>();

if (args.Contains("--seed"))
{
    await app.Services.SeedAsync();

    using (var connection = new SqlConnection(configuration2.GetConnectionString("HangfireConnection", "HangfireDB")))
    {
        connection.Open();

        using (var command = new SqlCommand(string.Format(
            @"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') 
                                    create database [{0}];
                      ", "HangfireDB"), connection))
        {
            command.ExecuteNonQuery();
        }
    }

    return;
}

app.Services.InitializeJobs();

app.Run();