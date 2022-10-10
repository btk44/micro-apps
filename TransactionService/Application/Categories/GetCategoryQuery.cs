using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Categories;

public class GetCategoryQuery: IRequest<CategoryDto> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
{
    private IApplicationDbContext _dbContext;
    private IMapper _categoryMapper;

    public GetCategoryQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _categoryMapper = mapper;    
    }

    public async Task<CategoryDto> Handle(GetCategoryQuery query, CancellationToken cancellationToken)
    {
        var categoryEntity = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Active && x.OwnerId == query.OwnerId && x.Id == query.Id);

        if(categoryEntity == null){
            return null;
        }

        return _categoryMapper.Map<CategoryDto>(categoryEntity);
    }
}