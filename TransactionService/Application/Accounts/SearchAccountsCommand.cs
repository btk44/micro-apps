using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Accounts;

public class SearchAccountsCommand: IRequest<List<AccountDto>> {
    public int OwnerId { get; set; }
    public int[] Currencies { get; set; }
    public string Name { get; set; }
    public double AmountFrom { get; set; } 
    public double AmountTo { get; set; }
    public int Id { get; set; }
    public bool Closed { get; set; }
}

public class SearchAccountsCommandHandler : IRequestHandler<SearchAccountsCommand, List<AccountDto>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _accountMapper;

    public SearchAccountsCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _accountMapper = mapper;
    }

    public async Task<List<AccountDto>> Handle(SearchAccountsCommand command, CancellationToken cancellationToken)
    {
        var accountQuery = _dbContext.Accounts
                                    .Where(x => x.Active != command.Closed &&
                                                x.OwnerId == command.OwnerId && 
                                                x.Amount >= command.AmountFrom && 
                                                x.Amount <= command.AmountTo);

        if (command.Currencies.Any()){
            accountQuery = accountQuery.Where(x => command.Currencies.Contains(x.CurrencyId));
        }

        if(!string.IsNullOrEmpty(command.Name)){
            accountQuery = accountQuery.Where(x => x.Name.ToLower().Contains(command.Name.ToLower()));
        }

        if(command.Id > 0){
            accountQuery = accountQuery.Where(x => x.Id == command.Id);
        }

        return await accountQuery.Select(x => _accountMapper.Map<AccountDto>(x)).ToListAsync();
    }
}