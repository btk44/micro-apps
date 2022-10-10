using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Transactions;

public class GetTransactionQuery: IRequest<TransactionDto> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionDto>
{
    private IApplicationDbContext _dbContext;
    private IMapper _transactionMapper;

    public GetTransactionQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _transactionMapper = mapper;    
    }

    public async Task<TransactionDto> Handle(GetTransactionQuery query, CancellationToken cancellationToken)
    {
        var transactionEntity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Active && x.OwnerId == query.OwnerId && x.Id == query.Id);

        if(transactionEntity == null){
            return null;
        }

        return _transactionMapper.Map<TransactionDto>(transactionEntity);
    }
}