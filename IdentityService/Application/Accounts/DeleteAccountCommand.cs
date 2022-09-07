using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteAccountCommand: IRequest<Result<bool>> {
    // for now we can remove only our account
    public int AccountId { get; set; }
}

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<bool>>
{
    private IApplicationDbContext _dbContext;

    public DeleteAccountCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Active && x.Id == command.AccountId);
        
        _dbContext.Accounts.Remove(account);
        if(await _dbContext.SaveChangesAsync() <= 0){
            return new Result<bool>(new AccountValidationException("Save error - please try again"));
        }

        return new Result<bool>(true);
    }
}
