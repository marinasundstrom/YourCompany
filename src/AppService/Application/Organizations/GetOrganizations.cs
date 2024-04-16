using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;

using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.Application.Organizations;

public record GetOrganizations(int Page = 1, int PageSize = 10, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemResult<Organization>>
{
    public class Handler(IRequestClient<IdentityManagement.Contracts.GetOrganizations> requestClient) : IRequestHandler<GetOrganizations, ItemResult<Organization>>
    {
        public async Task<ItemResult<Organization>> Handle(GetOrganizations request, CancellationToken cancellationToken)
        {
            var response = await requestClient.GetResponse<GetOrganizationsResponse>(new IdentityManagement.Contracts.GetOrganizations());
            var message = response.Message;

            return new ItemResult<Organization>(message.Items, message.Total);
        }
    }
}