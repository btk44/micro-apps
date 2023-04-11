using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Categories;

public class ProcessCategoriesCommand: IRequest<Either<List<CategoryDto>, CategoryValidationException>> {
    public int ProcessingUserId { get; set; }
    //public List<CategoryGroupDto> CategoryGroups { get; set; }
    public List<CategoryDto> Categories { get; set; }
}

public class ProcessCategoriesCommandHandler : IRequestHandler<ProcessCategoriesCommand, Either<List<CategoryDto>, CategoryValidationException>>
{
    private IApplicationDbContext _dbContext;
    private CategoryValidator _categoryValidator;
    private IMapper _categoryMapper;

    public ProcessCategoriesCommandHandler(IApplicationDbContext dbContext, IMapper categoryMapper){
        _dbContext = dbContext;
        _categoryValidator = new CategoryValidator();
        _categoryMapper = categoryMapper;
    }

    public async Task<Either<List<CategoryDto>, CategoryValidationException>> Handle(ProcessCategoriesCommand command, CancellationToken cancellationToken)
    {
        if(command.Categories.Any(x => x.OwnerId <= 0)){
            var incorrectCategoriesIds = string.Join(", ", command.Categories.Where(x => x.OwnerId <= 0).Select(x => x.Id));
            return new CategoryValidationException($"Incorrect owner id in accounts: { incorrectCategoriesIds }");
        }

        var categoryIdList = command.Categories.Where(x => x.Id > 0).Select(x => x.Id);
        var existingCategories = await _dbContext.Categories.Where(x => categoryIdList.Contains(x.Id)).ToListAsync();
        var existingIds = existingCategories.Select(x => x.Id);
        var missingCategories = categoryIdList.Except(existingIds);

        if(missingCategories.Any()){
            return new CategoryValidationException($"Missing accounts: { string.Join(", ", missingCategories) }");
        }

        var processedEntities = new List<CategoryEntity>();

        foreach(var commandCategory in command.Categories){
            CategoryEntity categoryEntity = existingCategories.FirstOrDefault(x => x.Id == commandCategory.Id);
            if(categoryEntity == null){
                categoryEntity = new CategoryEntity();
                _dbContext.Categories.Add(categoryEntity);
            }

            _categoryMapper.Map(commandCategory, categoryEntity);
            processedEntities.Add(categoryEntity);
        }

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new CategoryValidationException("Save error - please try again");
        }

        return processedEntities.Select(x => _categoryMapper.Map<CategoryDto>(x)).ToList();
    }
}