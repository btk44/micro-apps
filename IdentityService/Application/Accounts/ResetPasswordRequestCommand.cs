using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Tools;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Accounts;

public class ResetPasswordRequestCommand : IRequest<Either<bool, AccountValidationException>>  {
    public string Email { get; set; }   
}

public class ResetPasswordRequestCommandHandler : IRequestHandler<ResetPasswordRequestCommand, Either<bool, AccountValidationException>>
{
    private IApplicationDbContext _dbContext;
    private ITokenService _tokenService;

    public ResetPasswordRequestCommandHandler(IApplicationDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<Either<bool,AccountValidationException>> Handle(ResetPasswordRequestCommand command, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts
                                .Include(x => x.ResetPasswordTokens)
                                .FirstOrDefaultAsync(x => x.Active && x.Email == command.Email);

        if (account == null){
            return new AccountValidationException("Email does not exist");
        }

        account.ResetPasswordTokens.Add(new ResetPasswordTokenEntity() {
            Token = _tokenService.GenerateSimpleToken()
        });

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        // to do : send email to the account owner

        return true;
    }
}