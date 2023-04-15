using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Categories;

public class ProcessCategoriesCommand: IRequest<Either<List<CategoryGroupDto>, CategoryValidationException>> {
    public int ProcessingUserId { get; set; }
    public List<CategoryGroupDto> CategoryGroups { get; set; }
}

public class ProcessCategoriesCommandHandler : IRequestHandler<ProcessCategoriesCommand, Either<List<CategoryGroupDto>, CategoryValidationException>>
{
    private IApplicationDbContext _dbContext;
    private CategoryValidator _categoryValidator;
    private IMapper _categoryMapper;

    public ProcessCategoriesCommandHandler(IApplicationDbContext dbContext, IMapper categoryMapper){
        _dbContext = dbContext;
        _categoryValidator = new CategoryValidator();
        _categoryMapper = categoryMapper;
    }

    public async Task<Either<List<CategoryGroupDto>, CategoryValidationException>> Handle(ProcessCategoriesCommand command, CancellationToken cancellationToken)
    {
        if (command.CategoryGroups.Any(x => x.OwnerId <= 0)){
            var incorrectCategoryGroupIds = string.Join(", ", command.CategoryGroups.Where(x => x.OwnerId <= 0).Select(x => x.Id));
            return new CategoryValidationException($"Incorrect owner id in category groups: { incorrectCategoryGroupIds }");
        }

        foreach(var categoryGroup in command.CategoryGroups){
            if(categoryGroup.Categories.Any(x => x.OwnerId != categoryGroup.OwnerId)){
                var incorrectCategoriesIds = string.Join(", ", categoryGroup.Categories.Where(x => x.OwnerId != categoryGroup.OwnerId).Select(x => x.Id));
                return new CategoryValidationException($"Incorrect owner id in categories: { incorrectCategoriesIds }");
            }
        }

        var categoryGroupsIdList = command.CategoryGroups.Where(x => x.Id > 0).Select(x => x.Id);
        var existingCategoryGroups = await _dbContext.CategoryGroups
                                                .Include(x => x.Categories)
                                                .Where(x => categoryGroupsIdList.Contains(x.Id))
                                                .ToListAsync();
        var existingGroupIds = existingCategoryGroups.Select(x => x.Id);
        var missingGroups = categoryGroupsIdList.Except(existingGroupIds);

        if(missingGroups.Any()){
            return new CategoryValidationException($"Missing category groups: { string.Join(", ", missingGroups) }");
        }

        var categoryIdList = command.CategoryGroups.SelectMany(x => x.Categories).Where(x => x.Id > 0).Select(x => x.Id);
        var existingCategories = await _dbContext.Categories.Where(x => categoryIdList.Contains(x.Id)).ToListAsync();
        var existingIds = existingCategories.Select(x => x.Id);
        var missingCategories = categoryIdList.Except(existingIds);

        if(missingCategories.Any()){
            return new CategoryValidationException($"Missing categories: { string.Join(", ", missingCategories) }");
        }

        var existingCategoriesToDeleteIds = command.CategoryGroups.SelectMany(x => x.Categories.Where(y => !y.Active)).Select(x => x.Id);
        var thereAreTransactionsForDeletedCategory = await _dbContext.Transactions.AnyAsync(x => existingCategoriesToDeleteIds.Contains(x.CategoryId) && x.Active);
        if(thereAreTransactionsForDeletedCategory){
            return new CategoryValidationException($"There are active transactions for one or more of these categories: { string.Join(", ", existingCategoriesToDeleteIds) }");
        }

        var processedGroupEntities = new List<CategoryGroupEntity>();

        foreach(var commandCategoryGroup in command.CategoryGroups){
            CategoryGroupEntity categoryGroupEntity = existingCategoryGroups.FirstOrDefault(x => x.Id == commandCategoryGroup.Id);
            if (categoryGroupEntity == null){
                categoryGroupEntity = new CategoryGroupEntity();
                _dbContext.CategoryGroups.Add( categoryGroupEntity);
            }

            _categoryMapper.Map(commandCategoryGroup, categoryGroupEntity);
            processedGroupEntities.Add(categoryGroupEntity);

            foreach(var commandCategory in commandCategoryGroup.Categories){
                CategoryEntity categoryEntity = existingCategories.FirstOrDefault(x => x.Id == commandCategory.Id);
                if(categoryEntity == null){
                    categoryEntity = new CategoryEntity();
                    _dbContext.Categories.Add(categoryEntity);  // what if group is new and child is old? how to remove it from previous group
                }

                _categoryMapper.Map(commandCategory, categoryEntity);
                categoryGroupEntity.Categories.Add(categoryEntity);
            }
        }

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new CategoryValidationException("Save error - please try again");
        }

        var resultCategoryGroups = new List<CategoryGroupDto>();
        foreach(var processedCategoryGroup in processedGroupEntities){
            var categoryGroupDto = _categoryMapper.Map<CategoryGroupDto>(processedCategoryGroup);
            foreach(var category in processedCategoryGroup.Categories){
                categoryGroupDto.Categories.Add(_categoryMapper.Map<CategoryDto>(category));
            }
            resultCategoryGroups.Add(categoryGroupDto);
        }

        return resultCategoryGroups;
    }
}