using System.Globalization;
using CsvHelper;
using TransactionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;

public class Transaction{
    [CsvHelper.Configuration.Attributes.Index(0)]
    public DateTime Date { get; set; }
    [CsvHelper.Configuration.Attributes.Index(1)]
    public int AccountId { get; set; }
    [CsvHelper.Configuration.Attributes.Index(2)]
    public string AccountName { get; set; }
    [CsvHelper.Configuration.Attributes.Index(3)]
    public double Amount { get; set; }
    [CsvHelper.Configuration.Attributes.Index(7)]
    public int CategoryId { get; set; }
    [CsvHelper.Configuration.Attributes.Index(8)]
    public string CategoryName { get; set; }
    [CsvHelper.Configuration.Attributes.Index(4)]
    public string Payee { get; set; }
    [CsvHelper.Configuration.Attributes.Index(9)]
    public string Comment { get; set; }
}

public class Importer {
    private IApplicationDbContext _dbContext;

    public Importer(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ImportFromCsv(){
        IEnumerable<Transaction> records;

        using (var reader = new StreamReader("testdata1.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<Transaction>().ToList();
        }

        var accounts = await _dbContext.Accounts.ToDictionaryAsync(x => x.Name);
        var categories = await _dbContext.Categories.ToDictionaryAsync(x => x.Name);
        var currency = await _dbContext.Currencies.FirstOrDefaultAsync(x => x.Code == "PLN");

        var categoryGroup = new CategoryGroupEntity(){
            Name = "Default",
            OwnerId = 1
        };

        await _dbContext.CategoryGroups.AddAsync(categoryGroup);

        foreach(Transaction record in records){
            if (!accounts.ContainsKey(record.AccountName)){
                var newAccount = new AccountEntity() {
                    Name = record.AccountName,
                    Amount = 0,
                    Currency = currency, // change this manually later
                    OwnerId = 1
                };

                await _dbContext.Accounts.AddAsync(newAccount);
                accounts.Add(newAccount.Name, newAccount);
            }

            if(!categories.ContainsKey(record.CategoryName)){
                var newCategory = new CategoryEntity(){
                    Name = record.CategoryName,
                    OwnerId = 1,
                    CategoryGroup = categoryGroup
                };

                await _dbContext.Categories.AddAsync(newCategory);
                categories.Add(newCategory.Name, newCategory);
            }


            await _dbContext.Transactions.AddAsync(new TransactionEntity(){
                OwnerId = 1,
                Date = record.Date,
                Account = accounts[record.AccountName],
                Amount = record.Amount,
                Payee = record.Payee,
                Category = categories[record.CategoryName],
                Comment = record.Comment
            });
        }

        await _dbContext.SaveChangesAsync();

        var visualproperties = await _dbContext.VisualProperties.ToListAsync();

        foreach(var account in accounts){
            var accountEntity = account.Value;
            accountEntity.Amount = await _dbContext.Transactions.Where(x => x.AccountId == accountEntity.Id).SumAsync(x => x.Amount);
            if (!visualproperties.Any(x => x.ParentObjectId == accountEntity.Id && x.ParentObjectName == nameof(accountEntity))){
                await _dbContext.VisualProperties.AddAsync(new VisualPropertiesEntity(){
                    ParentObjectId = accountEntity.Id,
                    ParentObjectName = nameof(accountEntity)
                });
            }
        }

        foreach(var category in categories){
            var categoryEntity = category.Value;
            if (!visualproperties.Any(x => x.ParentObjectId == categoryEntity.Id && x.ParentObjectName == nameof(categoryEntity))){
                await _dbContext.VisualProperties.AddAsync(new VisualPropertiesEntity(){
                    ParentObjectId = categoryEntity.Id,
                    ParentObjectName = nameof(categoryEntity)
                });
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}