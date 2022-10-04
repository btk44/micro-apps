using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Transactions;

public class SearchTransactionsCommand: IRequest<Result<List<TransactionDto>>> {
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TransactionId { get; set; }
    public int OwnerId { get; set; }
    public double AmountFrom { get; set; } 
    public double AmountTo { get; set; }
    public string Payee{ get; set; }
    public List<int> Categories { get; set; }
    public string Comment { get; set; }
    public bool Removed { get; set; }
}

public class SearchTransactionsCommandHandler : IRequestHandler<SearchTransactionsCommand, Result<List<TransactionDto>>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _accountMapper;

    public SearchTransactionsCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _accountMapper = mapper;
    }

    public async Task<Result<List<TransactionDto>>> Handle(SearchTransactionsCommand command, CancellationToken cancellationToken)
    {
        var accountQuery = _dbContext.Transactions
                .Where(x => x.Active != command.Removed &&
                            x.OwnerId == command.OwnerId && 
                            x.Amount > command.AmountFrom && x.Amount < command.AmountTo);


        return await accountQuery.Select(x => _accountMapper.Map<TransactionDto>(x)).ToListAsync();
    }
}