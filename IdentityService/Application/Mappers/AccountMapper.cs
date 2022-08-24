using AutoMapper;
using IdentityService.Database.Entities;
using IdentityService.DataObjects;

namespace IdentityService.Application.Mappers;

public class AccountMapper: Profile {
    public AccountMapper()
    {
        CreateMap<AccountEntity, AccountDto>().ForMember(destDto => destDto.Password, opt => opt.Ignore());
        CreateMap<AccountDto, AccountEntity>();
    }
}