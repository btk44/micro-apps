using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using Shared.Tools;

namespace TransactionService.Application.Categories;

public class DeleteCategoryCommand: IRequest<Either<bool, CategoryValidationException>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class DeleteCategoryCommandHandler: IRequestHandler<DeleteCategoryCommand, Either<bool, CategoryValidationException>> {
    private IApplicationDbContext _dbContext;

    public DeleteCategoryCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
    }

    public async Task<Either<bool, CategoryValidationException>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken){
        var categoryEntity = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(categoryEntity == null){
            return true;
        }

        categoryEntity.Active = false;

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new CategoryValidationException("Save error - please try again");
        }

        return true;
    }
}