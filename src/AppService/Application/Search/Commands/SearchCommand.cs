using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;

namespace YourBrand.Application.Search.Commands;

public record SearchCommand(string SearchText,
        int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemResult<SearchResultItem>>
{
    public class SearchCommandHandler(IAppServiceContext context) : IRequestHandler<SearchCommand, ItemResult<SearchResultItem>>
    {
        public async Task<ItemResult<SearchResultItem>> Handle(SearchCommand request, CancellationToken cancellationToken)
        {
            var searchText = request.SearchText.Trim().ToLower();

            var query = context.Items.Where(i =>
                i.Name.Trim().ToLower().Contains(searchText)
                || i.Description.Trim().ToLower().Contains(searchText))
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var projectedQuery = query.Select(i => new SearchResultItem()
            {
                Title = i.Name,
                ResultType = SearchResultItemType.Item,
                ItemId = i.Id
            });

            if (request.SortBy is not null)
            {
                projectedQuery = projectedQuery.OrderBy(
                    request.SortBy,
                    request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            projectedQuery = projectedQuery.Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable();

            var resultItems = await projectedQuery.ToListAsync(cancellationToken);

            return new ItemResult<SearchResultItem>(
                resultItems,
                totalCount);
        }
    }
}