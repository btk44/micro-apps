using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Transactions;

public class SearchTransactionsCommand: IRequest<List<TransactionDto>> {
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public double AmountFrom { get; set; } 
    public double AmountTo { get; set; }
    public string Payee{ get; set; }
    public List<int> Categories { get; set; }
    public string Comment { get; set; }
    public bool Active { get; set; }
    public bool ActiveDefined { get; set; }
    public List<int> Accounts { get; set; }
    public int Take { get; set; }
    public int Offset { get; set; }
}

public class SearchTransactionsCommandHandler : IRequestHandler<SearchTransactionsCommand, List<TransactionDto>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _transactionMapper;

    public SearchTransactionsCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _transactionMapper = mapper;
    }

    public async Task<List<TransactionDto>> Handle(SearchTransactionsCommand command, CancellationToken cancellationToken)
    {
        var transactionQuery = _dbContext.Transactions
                .Where(x => x.OwnerId == command.OwnerId && 
                            x.Amount >= command.AmountFrom && 
                            x.Amount <= command.AmountTo && 
                            x.Date  >= command.DateFrom &&
                            x.Date <= command.DateTo);

        if( command.Id > 0){
            transactionQuery = transactionQuery.Where(x => x.Id == command.Id);
        }

        if(command.ActiveDefined){
            transactionQuery = transactionQuery.Where(x => x.Active == command.Active);
        }

        if(!string.IsNullOrEmpty(command.Payee)){
            transactionQuery = transactionQuery.Where(x => x.Payee.ToLower().Contains(command.Payee.ToLower()));
        }

        if(!string.IsNullOrEmpty(command.Comment)){
            transactionQuery = transactionQuery.Where(x => x.Comment.ToLower().Contains(command.Comment.ToLower()));
        }

        if(command.Categories.Any()){
            transactionQuery = transactionQuery.Where(x => command.Categories.Contains(x.CategoryId));
        }

        if(command.Accounts.Any()){
            transactionQuery = transactionQuery.Where(x => command.Accounts.Contains(x.AccountId));
        }

        return await transactionQuery.Select(x => _transactionMapper.Map<TransactionDto>(x)).ToListAsync();
    }
}