using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public sealed class GetOrganizationConsumer(IMediator mediator) : IConsumer<GetOrganization>
{
    public async Task Consume(ConsumeContext<GetOrganization> context)
    {
        var message = context.Message;

        var organization = await mediator.Send(new YourBrand.IdentityManagement.Application.Organizations.Queries.GetOrganizationQuery(message.OrganizationId));

        await context.RespondAsync(new GetOrganizationResponse(organization.Id, organization.Tenant.Id, organization.Name, organization.FriendlyName));
    }
}
