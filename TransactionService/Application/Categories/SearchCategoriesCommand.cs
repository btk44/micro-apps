using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Categories;

public class SearchCategoriesCommand: IRequest<List<CategoryDto>> {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public string CategoryGroupName { get; set; }
    public bool Deleted { get; set; }
}

public class SearchCategoriesCommandHandler : IRequestHandler<SearchCategoriesCommand, List<CategoryDto>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _categoryMapper;

    public SearchCategoriesCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _categoryMapper = mapper;
    }

    public async Task<List<CategoryDto>> Handle(SearchCategoriesCommand command, CancellationToken cancellationToken)
    {
        var categoryQuery = _dbContext.Categories
                .Where(x => x.Active != command.Deleted &&
                            x.OwnerId == command.OwnerId);

        if(!string.IsNullOrEmpty(command.Name)){
            categoryQuery = categoryQuery.Where(x => x.Name.ToLower().Contains(command.Name.ToLower()));
        }

        if(command.Id > 0){
            categoryQuery = categoryQuery.Where(x => x.Id == command.Id);
        }

        if(string.IsNullOrEmpty(command.CategoryGroupName)){
            categoryQuery = categoryQuery
                    .Where(x => x.CategoryGroupName.ToLower().Contains(command.CategoryGroupName.ToLower()));
        }

        return await categoryQuery.Select(x => _categoryMapper.Map<CategoryDto>(x)).ToListAsync();
    }
}