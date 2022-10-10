using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Accounts;

public class GetAccountQuery: IRequest<Result<AccountDto>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Result<AccountDto>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _accountMapper;

    public GetAccountQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _accountMapper = mapper;    
    }

    public async Task<Result<AccountDto>> Handle(GetAccountQuery query, CancellationToken cancellationToken)
    {
        var accountEntity = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Active && x.OwnerId == query.OwnerId && x.Id == query.Id);

        if(accountEntity == null){
            return new AccountValidationException("Account does not exist");
        }

        return _accountMapper.Map<AccountDto>(accountEntity);
    }
}