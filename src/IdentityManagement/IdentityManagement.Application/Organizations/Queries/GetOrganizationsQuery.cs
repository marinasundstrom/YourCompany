
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Application.Common.Models;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Application.Organizations.Queries;

public record GetOrganizationsQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, IdentityManagement.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<OrganizationDto>>
{
    public class GetOrganizationsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetOrganizationsQuery, ItemsResult<OrganizationDto>>
    {
        public async Task<ItemsResult<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TC TenantId: " + tenantContext.TenantId);

            var query = context.Organizations
                .Include(x => x.Tenant)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityManagement.Application.Common.Models.SortDirection.Desc ? IdentityManagement.Application.SortDirection.Descending : IdentityManagement.Application.SortDirection.Ascending);
            }

            var organizations = await query
                .Skip(request.PageSize * (request.Page - 1))
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = organizations.Select(organization => organization.ToDto());

            return new ItemsResult<OrganizationDto>(dtos, totalItems);
        }
    }
}