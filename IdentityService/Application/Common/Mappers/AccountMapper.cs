using AutoMapper;
using IdentityService.Application.Common.Models;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Common.Mappers;

public class AccountMapper: Profile {
    public AccountMapper()
    {
        CreateMap<AccountEntity, AccountDto>().ForMember(destDto => destDto.Password, opt => opt.Ignore());
        CreateMap<AccountDto, AccountEntity>();
    }
}