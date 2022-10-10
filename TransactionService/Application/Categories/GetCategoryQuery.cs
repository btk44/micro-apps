using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Categories;

public class GetCategoryQuery: IRequest<Result<CategoryDto>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<CategoryDto>>
{
    private IApplicationDbContext _dbContext;
    private IMapper _categoryMapper;

    public GetCategoryQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _categoryMapper = mapper;    
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryQuery query, CancellationToken cancellationToken)
    {
        var categoryEntity = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Active && x.OwnerId == query.OwnerId && x.Id == query.Id);

        if(categoryEntity == null){
            return new CategoryValidationException("Category does not exist");
        }

        return _categoryMapper.Map<CategoryDto>(categoryEntity);
    }
}