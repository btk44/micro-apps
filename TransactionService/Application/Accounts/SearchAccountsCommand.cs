using MediatR;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Accounts;

public class SearchAccountsCommand: IRequest<Result<List<AccountDto>>> {
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int AccountId { get; set; }
    public int OwnerId { get; set; }
    public double AmountFrom { get; set; } 
    public double AmountTo { get; set; }
    public string Payee{ get; set; }
    public List<int> Categories { get; set; }
    public string Comment { get; set; }
}

public class SearchAccountsCommandHandler : IRequestHandler<SearchAccountsCommand, Result<List<AccountDto>>>
{
    public Task<Result<List<AccountDto>>> Handle(SearchAccountsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}