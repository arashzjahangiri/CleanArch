using FluentValidation;

namespace Shop.Query.Application.Customer.Queries;

public class GetAllCustomerQueryValidator : AbstractValidator<GetAllCustomerQuery>
{
    public GetAllCustomerQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, GetAllCustomerQuery.MaxPageSize);
    }
}
