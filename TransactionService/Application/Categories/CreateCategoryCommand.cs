using AutoMapper;
using MediatR;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Categories;

public class CreateCategoryCommand: IRequest<Result<CategoryDto>> {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int ParentCategoryId { get; set; }
}

public class CreateCategoryCommandHandler: IRequestHandler<CreateCategoryCommand, Result<CategoryDto>> {
    private IApplicationDbContext _dbContext;
    private CategoryValidator _categoryValidator;
    private IMapper _categoryMapper;

    public CreateCategoryCommandHandler(IApplicationDbContext dbContext, IMapper categoryMapper){
        _dbContext = dbContext;
        _categoryValidator = new CategoryValidator();
        _categoryMapper = categoryMapper; // consider moving mapping into Dto file?
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken){
        if(!_categoryValidator.IsNameValid(command.Name)){
            return new CategoryValidationException("Incorrect category name");
        }

        if(command.OwnerId <= 0){
            return new CategoryValidationException("Incorrect owner id");
        }

        var categoryEntity = new CategoryEntity(){
            Name = command.Name,
            ParentCategoryId = command.ParentCategoryId,
            OwnerId = command.OwnerId
        };

        _dbContext.Categories.Add(categoryEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new CategoryValidationException("Save error - please try again");
        }

        var dto = _categoryMapper.Map<CategoryDto>(categoryEntity);

        return dto;
    }
}