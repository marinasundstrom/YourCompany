namespace YourBrand.IdentityManagement.Contracts;

public record Organization(string Id, string Name, string? FriendlyName);

public record CreateOrganization
{
    public string Name { get; init; }
    public string? FriendlyName { get; init; }
}

public record GetOrganizations();

public record GetOrganizationsResponse(IEnumerable<Organization> Items, int Total);

public record GetOrganization(string OrganizationId, string RequestedById);

public record GetOrganizationResponse(string Id, string TenantId, string Name, string? FriendlyName);

public record CreateOrganizationResponse(string OrganizationId, string Name, string? FriendlyName);

public record OrganizationCreated(string OrganizationId, string TenantId, string Name);

public record OrganizationUpdated(string OrganizationId, string Name);

public record OrganizationDeleted(string OrganizationId);

public record AddOrganizationUser(string OrganizationId, string UserId);

public record OrganizationUserAdded(string TenantId, string OrganizationId, string UserId);

public record RemoveOrganizationUser(string OrganizationId, string UserId);

public record OrganizationUserRemoved(string OrganizationId, string UserId);