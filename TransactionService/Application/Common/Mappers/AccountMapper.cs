using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class AccountMapper: Profile {
    public AccountMapper()
    {
        CreateMap<AccountEntity, AccountDto>();
        CreateMap<AccountDto, AccountEntity>();
    }
}