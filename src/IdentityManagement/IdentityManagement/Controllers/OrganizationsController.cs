using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using YourBrand.IdentityManagement.Application.Common.Models;
using YourBrand.IdentityManagement.Application.Organizations;
using YourBrand.IdentityManagement.Application.Organizations.Commands;
using YourBrand.IdentityManagement.Application.Organizations.Queries;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement;

[Route("[controller]")]
[ApiController]
[Authorize]
public class OrganizationsController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<OrganizationDto>>> GetOrganizations(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, IdentityManagement.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetOrganizationsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationDto>> GetOrganization(string id, CancellationToken cancellationToken)
    {
        var organization = await mediator.Send(new GetOrganizationQuery(id), cancellationToken);

        if (organization is null)
        {
            return NotFound();
        }

        return Ok(organization);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationDto>> CreateOrganization(CreateOrganizationDto createOrganizationDto, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await mediator.Send(new CreateOrganizationCommand(createOrganizationDto.Name, createOrganizationDto.FriendlyName), cancellationToken);

            return Ok(organization);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("{id}/Details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationDto>> UpdateOrganization(string id, UpdateOrganizationDto updateOrganizationDto, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await mediator.Send(new UpdateOrganizationCommand(id, updateOrganizationDto.Name), cancellationToken);

            return Ok(organization);
        }
        catch (OrganizationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteOrganization(string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteOrganizationCommand(id), cancellationToken);

            return Ok();
        }
        catch (OrganizationNotFoundException)
        {
            return NotFound();
        }
    }
}

public record class CreateOrganizationDto(string Name, string? FriendlyName);

public record class UpdateOrganizationDto(string Name);