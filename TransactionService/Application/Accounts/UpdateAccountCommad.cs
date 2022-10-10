using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Accounts;

public class UpdateAccountCommand: IRequest<Result<bool>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int CurrencyId { get; set; }
}

public class UpdateAccountCommandHandler: IRequestHandler<UpdateAccountCommand, Result<bool>> {
    private IApplicationDbContext _dbContext;
    private AccountValidator _accountValidator;

    public UpdateAccountCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
        _accountValidator = new AccountValidator();
    }

    public async Task<Result<bool>> Handle(UpdateAccountCommand command, CancellationToken cancellationToken){
        if(!_accountValidator.IsNameValid(command.Name)){
            return new AccountValidationException("Incorrect account name");
        }

        if(command.OwnerId <= 0){
            return new AccountValidationException("Incorrect owner id");
        }
                
        var currency = await _dbContext.Currencies.FirstOrDefaultAsync(x => x.Active && x.Id == command.CurrencyId);

        if (currency == null){
            return new AccountValidationException("Unsupported currency");
        }

        var accountEntity = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(accountEntity == null){
            return new AccountValidationException("Account does not exist");
        }

        accountEntity.Name = command.Name;
        accountEntity.CurrencyId = command.CurrencyId;

        _dbContext.Accounts.Add(accountEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        return true;
    }
}