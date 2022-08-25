using AutoMapper;
using IdentityService.Application.Dtos;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Mappers;

public class AccountMapper: Profile {
    public AccountMapper()
    {
        CreateMap<AccountEntity, AccountDto>().ForMember(destDto => destDto.Password, opt => opt.Ignore());
        CreateMap<AccountDto, AccountEntity>();
    }
}