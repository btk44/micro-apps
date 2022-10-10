using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using Shared.Tools;

namespace TransactionService.Application.Accounts;

public class DeleteAccountCommand: IRequest<Either<bool, AccountValidationException>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class DeleteAccountCommandHandler: IRequestHandler<DeleteAccountCommand, Either<bool, AccountValidationException>> {
    private IApplicationDbContext _dbContext;

    public DeleteAccountCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
    }

    public async Task<Either<bool, AccountValidationException>> Handle(DeleteAccountCommand command, CancellationToken cancellationToken){
        var accountEntity = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(accountEntity == null){
            return true;
        }

        accountEntity.Active = false;

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        return true;
    }
}