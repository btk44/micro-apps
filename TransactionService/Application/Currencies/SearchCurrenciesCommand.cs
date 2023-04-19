using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;

namespace TransactionService.Application.Currencies;

public class SearchCurrenciesCommand: IRequest<List<CurrencyDto>> {
    public string Code { get; set; }
    public int Id { get; set; }
    public string Description { get; set; }
}

public class SearchCurrenciesCommandHandler : IRequestHandler<SearchCurrenciesCommand, List<CurrencyDto>>
{
    private IApplicationDbContext _dbContext;

    public SearchCurrenciesCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CurrencyDto>> Handle(SearchCurrenciesCommand command, CancellationToken cancellationToken)
    {
        var currencyQuery = _dbContext.Currencies.Where(x => true);

        if(!string.IsNullOrEmpty(command.Code)){
            currencyQuery = currencyQuery.Where(x => x.Code.ToLower().Contains(command.Code.ToLower()));
        }

        if(command.Id > 0){
            currencyQuery = currencyQuery.Where(x => x.Id == command.Id);
        }

        if(!string.IsNullOrEmpty(command.Description)){
            currencyQuery = currencyQuery.Where(x => x.Description.ToLower().Contains(command.Description.ToLower()));
        }

        return await currencyQuery.Select(x => new CurrencyDto() {
            Id = x.Id, Code = x.Code, Description = x.Description
        }).ToListAsync();
    }
}