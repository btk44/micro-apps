using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteAccountCommand: IRequest<Either<bool,AccountValidationException>> {
    // for now we can remove only our account
    public int AccountId { get; set; }
}

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Either<bool,AccountValidationException>>
{
    private IApplicationDbContext _dbContext;

    public DeleteAccountCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Either<bool, AccountValidationException>> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Active && x.Id == command.AccountId);
        
        _dbContext.Accounts.Remove(account);
        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        return true;
    }
}
