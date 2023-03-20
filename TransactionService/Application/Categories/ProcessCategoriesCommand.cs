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
        return new CategoryValidationException("No implementation yet");
    }
}