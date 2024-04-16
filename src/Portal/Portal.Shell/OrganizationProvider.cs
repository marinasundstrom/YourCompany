using Blazored.LocalStorage;

using YourBrand.AppService.Client;
using YourBrand.Portal.Services;

namespace YourBrand.Portal;

public sealed class OrganizationProvider(IOrganizationsClient organizationsClient, ILocalStorageService localStorageService) : IOrganizationProvider
{
    IEnumerable<Portal.Services.Organization>? _organizations;
    private Services.Organization? _currentOrganization;

    public async Task<IEnumerable<Portal.Services.Organization>> GetAvailableOrganizationsAsync(CancellationToken cancellationToken = default)
    {
        _organizations = (await organizationsClient.GetOrganizationsAsync(0, null, null, null, cancellationToken)).Items
            .Select(x => new Portal.Services.Organization(x.Id, x.Name, x.FriendlyName));

        _currentOrganization = await GetCurrentOrganizationAsync(cancellationToken);

        return _organizations;
    }

    public async Task<Portal.Services.Organization?> GetCurrentOrganizationAsync(CancellationToken cancellationToken)
    {
        if (_organizations is null)
        {
            await GetAvailableOrganizationsAsync();
        }

        if (_currentOrganization is null)
        {
            var organizationId = await localStorageService.GetItemAsStringAsync("organizationId", cancellationToken);
            _currentOrganization = _organizations!.FirstOrDefault(x => x.Id == organizationId);
        }

        return _currentOrganization;
    }

    public async Task SetCurrentOrganization(string organizationId, CancellationToken cancellationToken = default)
    {
        if (_organizations is null)
        {
            await GetAvailableOrganizationsAsync();
        }

        _currentOrganization = _organizations!.FirstOrDefault(x => x.Id == organizationId);

        await localStorageService.SetItemAsStringAsync("organizationId", organizationId, cancellationToken);

        CurrentOrganizationChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? CurrentOrganizationChanged;
}