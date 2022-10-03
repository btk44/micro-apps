using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Categories;

public class UpdateCategoryCommand: IRequest<Result<bool>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int ParentCategoryId { get; set; }
}

public class UpdateCategoryCommandHandler: IRequestHandler<UpdateCategoryCommand, Result<bool>> {
    private IApplicationDbContext _dbContext;
    private CategoryValidator _categoryValidator;

    public UpdateCategoryCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
        _categoryValidator = new CategoryValidator();
    }

    public async Task<Result<bool>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken){
        if(!_categoryValidator.IsNameValid(command.Name)){
            return new CategoryValidationException("Incorrect category name");
        }

        if(command.OwnerId <= 0){
            return new CategoryValidationException("Incorrect owner id");
        }

        var categoryEntity = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(categoryEntity == null){
            return new CategoryValidationException("Category does not exist");
        }

        categoryEntity.Name = command.Name;
        categoryEntity.ParentCategoryId = command.ParentCategoryId;

        _dbContext.Categories.Add(categoryEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new CategoryValidationException("Save error - please try again");
        }

        return true;
    }
}