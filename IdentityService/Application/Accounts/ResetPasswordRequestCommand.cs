using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Tools;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Accounts;

public class ResetPasswordRequestCommand : IRequest<Result<bool>>  {
    public string Email { get; set; }   
}

public class ResetPasswordRequestCommandHandler : IRequestHandler<ResetPasswordRequestCommand, Result<bool>>
{
    private IApplicationDbContext _dbContext;
    private ITokenService _tokenService;

    public ResetPasswordRequestCommandHandler(IApplicationDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<Result<bool>> Handle(ResetPasswordRequestCommand command, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts
                                .Include(x => x.ResetPasswordTokens)
                                .FirstOrDefaultAsync(x => x.Active && x.Email == command.Email);

        if (account == null){
            return new Result<bool>(new AccessViolationException("Email does not exist"));
        }

        account.ResetPasswordTokens.Add(new ResetPasswordTokenEntity() {
            Token = _tokenService.GenerateSimpleToken()
        });

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new Result<bool>(new AccountValidationException("Save error - please try again"));
        }

        // to do : send email to the account owner

        return new Result<bool>(true);
    }
}