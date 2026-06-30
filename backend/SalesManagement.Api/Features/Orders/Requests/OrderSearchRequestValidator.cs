using FluentValidation;

namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class OrderSearchRequestValidator
    : AbstractValidator<OrderSearchRequest>
{
    public OrderSearchRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}