using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Categories;

public class SearchCategoriesCommand: IRequest<Result<List<CategoryDto>>> {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public int ParentCategoryId {get; set; }
    public bool Closed { get; set; }
}

public class SearchCategoriesCommandHandler : IRequestHandler<SearchCategoriesCommand, Result<List<CategoryDto>>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _categoryMapper;

    public SearchCategoriesCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _categoryMapper = mapper;
    }

    public async Task<Result<List<CategoryDto>>> Handle(SearchCategoriesCommand command, CancellationToken cancellationToken)
    {
        var categoryQuery = _dbContext.Categories
                .Where(x => x.Active != command.Closed &&
                            x.OwnerId == command.OwnerId);

        if(!string.IsNullOrEmpty(command.Name)){
            categoryQuery.Where(x => x.Name.Contains(command.Name));
        }

        if(command.Id > 0){
            categoryQuery.Where(x => x.Id == command.Id);
        }

        if(command.ParentCategoryId > 0){
            categoryQuery.Where(x => x.ParentCategoryId == command.ParentCategoryId);
        }

        return await categoryQuery.Select(x => _categoryMapper.Map<CategoryDto>(x)).ToListAsync();
    }
}