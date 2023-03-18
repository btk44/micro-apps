using AutoMapper;
using MediatR;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Categories;

public class ProcessCategoriesCommand: IRequest<Either<List<CategoryDto>, CategoryValidationException>> {
    public int ProcessingUserId { get; set; }
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
        List<int> incorrectCategoriesIdList = new List<int>();
        CheckOwnerId(command.Categories, out incorrectCategoriesIdList);
        if(incorrectCategoriesIdList.Any()){
            return new CategoryValidationException($"Incorrect owner id in categories: { string.Join(", ", incorrectCategoriesIdList) }");
        }

        

        return new CategoryValidationException("No implementation yet");
    }

    private void CheckOwnerId(List<CategoryDto> categories, out List<int> incorrectCategoriesIdList){
        incorrectCategoriesIdList = new List<int>();

        foreach(var category in categories){
            if(category.OwnerId <= 0){
                incorrectCategoriesIdList.Add(category.Id);
            }

            if(category.SubCategories.Any()){
                CheckOwnerId(category.SubCategories, out incorrectCategoriesIdList);
            }
        }
    }
}