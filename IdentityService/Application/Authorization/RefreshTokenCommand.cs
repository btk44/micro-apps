using IdentityService.Application.Common;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Models;
using MediatR;

namespace IdentityService.Application.Authorization;

public class RefreshTokenCommand: IRequest<Result<TokenDataDto>> {
    public string RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler: IRequestHandler<RefreshTokenCommand, Result<TokenDataDto>> {
    private IApplicationDbContext _dbContext;
    private AuthValidator _authValidator;

    public RefreshTokenCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
        _authValidator = new AuthValidator();
    }

    public async Task<Result<TokenDataDto>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(command.RefreshToken)){
            return new Result<TokenDataDto>(new AppException("no token provided"));
        }

        // get token claims here to get db token
        

        await _dbContext.SaveChangesAsync();
        return new Result<TokenDataDto>(new TokenDataDto());
    }
}