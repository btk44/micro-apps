using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Transactions;

public class SearchTransactionsCommand: IRequest<List<TransactionDto>> {
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
                .Where(x => x.Active != command.Removed &&
                            x.OwnerId == command.OwnerId && 
                            x.Amount > command.AmountFrom && x.Amount < command.AmountTo);


        return await transactionQuery.Select(x => _transactionMapper.Map<TransactionDto>(x)).ToListAsync();
    }
}