using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Accounts;

public class ProcessAccountsCommand: IRequest<Either<List<AccountDto>, AccountValidationException>> {
    public int ProcessingUserId { get; set; }
    public List<AccountDto> Accounts { get; set; }
}

public class ProcessAccountsCommandHandler : IRequestHandler<ProcessAccountsCommand, Either<List<AccountDto>, AccountValidationException>>
{
    private IApplicationDbContext _dbContext;
    private AccountValidator _accountValidator;
    private IMapper _accountMapper;

    public ProcessAccountsCommandHandler(IApplicationDbContext dbContext, IMapper accountMapper)
    {
        _dbContext = dbContext;
        _accountValidator = new AccountValidator();
        _accountMapper = accountMapper; // consider moving mapping into Dto file?
    }

    public async Task<Either<List<AccountDto>, AccountValidationException>> Handle(ProcessAccountsCommand command, CancellationToken cancellationToken)
    {
        if(command.Accounts.Any(x => x.OwnerId <= 0)){
            var incorrectAccountIds = string.Join(", ", command.Accounts.Where(x => x.OwnerId <= 0).Select(x => x.Id));
            return new AccountValidationException($"Incorrect owner id in accounts: { incorrectAccountIds }");
        }

        var currencyIdList = command.Accounts.Select(x => x.CurrencyId).Distinct();
        var accountIdList = command.Accounts.Where(x => x.Id > 0).Select(x => x.Id);

        var existingAccounts = await _dbContext.Accounts.Where(x => accountIdList.Contains(x.Id)).ToListAsync();
        var existingIds = existingAccounts.Select(x => x.Id);
        var missingAccounts = accountIdList.Except(existingIds);

        if(missingAccounts.Any()){
            return new AccountValidationException($"Missing accounts: { string.Join(", ", missingAccounts) }");
        }

        var currencies = await _dbContext.Currencies.Where(x => currencyIdList.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var processedEntities = new List<AccountEntity>();

        foreach(var commandAccount in command.Accounts){
            CurrencyEntity currency;
            
            if(!currencies.TryGetValue(commandAccount.CurrencyId, out currency)){
                return new AccountValidationException($"Currency does not exist (in account): { commandAccount.Id }");
            }

            AccountEntity accountEntity = existingAccounts.FirstOrDefault(x => x.Id == commandAccount.Id);
            if(accountEntity == null){
                accountEntity = new AccountEntity();
                _dbContext.Accounts.Add(accountEntity);
            }

            _accountMapper.Map(commandAccount, accountEntity);
            processedEntities.Add(accountEntity);
        }
        
        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        return processedEntities.Select(x => _accountMapper.Map<AccountDto>(x)).ToList();  
    }
}