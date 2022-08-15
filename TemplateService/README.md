## Database setup

Remember to open terminal and run command:
```
dotnet ef database update
```
And each time you change entities or DatabaseContext class run command:
```
dotnet ef migrations add NAME_OF_MIGRATION
```
More information here:
[Migrations Overview](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)