using AutoMapper;
using MediatR;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;
using Shared.Tools;
using Microsoft.EntityFrameworkCore;

namespace TransactionService.Application.Categories;

public class CreateCategoryCommand: IRequest<Either<CategoryDto, CategoryValidationException>> {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int ParentCategoryId { get; set; }
}

public class CreateCategoryCommandHandler: IRequestHandler<CreateCategoryCommand, Either<CategoryDto, CategoryValidationException>> {
    private IApplicationDbContext _dbContext;
    private CategoryValidator _categoryValidator;
    private IMapper _categoryMapper;

    public CreateCategoryCommandHandler(IApplicationDbContext dbContext, IMapper categoryMapper){
        _dbContext = dbContext;
        _categoryValidator = new CategoryValidator();
        _categoryMapper = categoryMapper; // consider moving mapping into Dto file?
    }

    public async Task<Either<CategoryDto, CategoryValidationException>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken){
        if(!_categoryValidator.IsNameValid(command.Name)){
            return new CategoryValidationException("Incorrect category name");
        }

        if(command.OwnerId <= 0){
            return new CategoryValidationException("Incorrect owner id");
        }

        var parentCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Active && x.Id == command.ParentCategoryId);

        var categoryEntity = new CategoryEntity(){
            Name = command.Name,
            OwnerId = command.OwnerId,
            ParentCategory = parentCategory
        };

        _dbContext.Categories.Add(categoryEntity);      
        if(await _dbContext.SaveChangesAsync() <= 0){
            return new CategoryValidationException("Save error - please try again");
        }

        var dto = _categoryMapper.Map<CategoryDto>(categoryEntity);

        return dto;
    }
}