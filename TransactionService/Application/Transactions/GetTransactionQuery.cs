using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Transactions;

public class GetTransactionQuery: IRequest<Result<TransactionDto>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, Result<TransactionDto>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _transactionMapper;

    public GetTransactionQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _transactionMapper = mapper;    
    }

    public async Task<Result<TransactionDto>> Handle(GetTransactionQuery query, CancellationToken cancellationToken)
    {
        var transactionEntity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Active && x.OwnerId == query.OwnerId && x.Id == query.Id);

        if(transactionEntity == null){
            return new TransactionValidationException("Transaction does not exist");
        }

        return _transactionMapper.Map<TransactionDto>(transactionEntity);
    }
}