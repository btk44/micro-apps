using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Accounts;

public class GetAccountQuery: IRequest<AccountDto> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
{
    private IApplicationDbContext _dbContext;
    private IMapper _accountMapper;

    public GetAccountQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _accountMapper = mapper;    
    }

    public async Task<AccountDto> Handle(GetAccountQuery query, CancellationToken cancellationToken)
    {
        var accountEntity = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Active && x.OwnerId == query.OwnerId && x.Id == query.Id);

        if(accountEntity == null){
            return null;
        }

        return _accountMapper.Map<AccountDto>(accountEntity);
    }
}