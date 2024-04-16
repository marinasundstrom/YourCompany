using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.Common.Models;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class OrganizationsController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<ActionResult<ItemResult<Organization>>> GetOrganizations(
        int page = 1, int pageSize = 5, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new Application.Organizations.GetOrganizations(page, pageSize, sortBy, sortDirection), cancellationToken));
    }
}