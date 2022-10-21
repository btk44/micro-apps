# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /TransactionService
    
# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out
    
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /TransactionService
COPY --from=build-env /TransactionService/out .
ENTRYPOINT ["dotnet", "TransactionService.dll"]