using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public sealed class GetOrganizationsConsumer(IMediator mediator) : IConsumer<GetOrganizations>
{
    public async Task Consume(ConsumeContext<GetOrganizations> context)
    {
        var message = context.Message;

        var result = await mediator.Send(new YourBrand.IdentityManagement.Application.Organizations.Queries.GetOrganizationsQuery(1, 10));

        await context.RespondAsync(new GetOrganizationsResponse(result.Items.Select(x => new Organization(x.Id, x.Name, x.FriendlyName)), result.TotalItems));
    }
}