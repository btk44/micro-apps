using Microsoft.EntityFrameworkCore;
using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure;

public class ApplicationDbContextInitialiser{
    private ApplicationDbContext _dbContext;

    public ApplicationDbContextInitialiser(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Migrate(){
        await _dbContext.Database.MigrateAsync();
        await InsertData();  // to do: remove it later?
    }

    private async Task InsertData(){
        if(!_dbContext.Accounts.Any()){
            Console.WriteLine("=== Inserting currency data ===");

            var currencies = new List<CurrencyEntity>() {
                new CurrencyEntity() { Description = "United States dollar", Code = "USD" },
                new CurrencyEntity() { Description = "Euro", Code = "EUR" },
                new CurrencyEntity() { Description = "Japanese yen", Code = "JPY" },
                new CurrencyEntity() { Description = "Sterling", Code = "GBP" },
                new CurrencyEntity() { Description = "Australian dollar", Code = "AUD" },
                new CurrencyEntity() { Description = "Canadian dollar", Code = "CAD" },
                new CurrencyEntity() { Description = "Swiss franc", Code = "CHF" },
                new CurrencyEntity() { Description = "Renminbi", Code = "CNY" },
                new CurrencyEntity() { Description = "Hong Kong dollar", Code = "HKD" },
                new CurrencyEntity() { Description = "New Zealand dollar", Code = "NZD" },
                new CurrencyEntity() { Description = " Swedish krona", Code = "SEK" },
                new CurrencyEntity() { Description = "South Korean won", Code = "KRW" },
                new CurrencyEntity() { Description = "Singapore dollar", Code = "SGD" },
                new CurrencyEntity() { Description = "Norwegian krone", Code = "NOK" },
                new CurrencyEntity() { Description = "Mexican peso", Code = "MXN" },
                new CurrencyEntity() { Description = "Indian rupee", Code = "INR" },
                new CurrencyEntity() { Description = "Russian ruble", Code = "RUB" },
                new CurrencyEntity() { Description = "South African rand", Code = "ZAR" },
                new CurrencyEntity() { Description = "Turkish lira", Code = "TRY" },
                new CurrencyEntity() { Description = "Brazilian real", Code = "BRL" },
                new CurrencyEntity() { Description = "New Taiwan dollar", Code = "TWD" },
                new CurrencyEntity() { Description = "Danish krone", Code = "DKK" },
                new CurrencyEntity() { Description = "Polish złoty", Code = "PLN" },
                new CurrencyEntity() { Description = "Thai baht", Code = "THB" },
                new CurrencyEntity() { Description = "Indonesian rupiah", Code = "IDR" },
                new CurrencyEntity() { Description = "Hungarian forint", Code = "HUF" },
                new CurrencyEntity() { Description = "Czech koruna", Code = "CZK" },
                new CurrencyEntity() { Description = "Israeli new shekel", Code = "ILS" },
                new CurrencyEntity() { Description = "Chilean peso", Code = "CLP" },
                new CurrencyEntity() { Description = "Philippine peso", Code = "PHP" },
                new CurrencyEntity() { Description = "UAE dirham", Code = "AED" },
                new CurrencyEntity() { Description = "Colombian peso", Code = "COP" },
                new CurrencyEntity() { Description = "Saudi riyal", Code = "SAR" },
                new CurrencyEntity() { Description = "Malaysian ringgit", Code = "MYR" },
                new CurrencyEntity() { Description = "Romanian leu", Code = "RON" }
            };

            _dbContext.Currencies.AddRange(currencies);

            await _dbContext.SaveChangesAsync();
            return;
        }

        Console.WriteLine("=== Currency data already inserted ===");
    }
}