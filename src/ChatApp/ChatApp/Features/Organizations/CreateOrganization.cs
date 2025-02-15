using FluentValidation;

using MediatR;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.Entities;
using YourBrand.ChatApp.Domain.Repositories;

namespace YourBrand.ChatApp.Features.Organizations;

public record CreateOrganization(string Id, string Name, string TenantId) : IRequest<Result<OrganizationDto>>
{
    public class Validator : AbstractValidator<CreateOrganization>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateOrganization, Result<OrganizationDto>>
    {
        public async Task<Result<OrganizationDto>> Handle(CreateOrganization request, CancellationToken cancellationToken)
        {
            organizationRepository.Add(new Organization(request.Id, request.Name)
            {
                TenantId = request.TenantId
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var organization = await organizationRepository.FindByIdAsync(request.Id, cancellationToken);

            if (organization is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            return Result.SuccessWith(organization.ToDto2());
        }
    }
}