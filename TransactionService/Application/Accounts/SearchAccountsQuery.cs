using MediatR;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Accounts;

public class SearchAccountsQuery: IRequest<Result<List<AccountDto>>> {
    // to do 
}

public class SearchAccountsQueryHandler : IRequestHandler<SearchAccountsQuery, Result<List<AccountDto>>>
{
    public Task<Result<List<AccountDto>>> Handle(SearchAccountsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}