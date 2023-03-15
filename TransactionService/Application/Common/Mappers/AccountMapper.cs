using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class AccountMapper: Profile {
    public AccountMapper()
    {
        CreateMap<AccountEntity, AccountDto>()
            .ForMember(dest => dest.Deleted, opt => opt.MapFrom(src => !src.Active))
            .ForPath(dest => dest.Amount, opt => opt.MapFrom(src => src.AdditionalInfo.Amount));
        CreateMap<AccountDto, AccountEntity>()
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => !src.Deleted))
            .ForPath(dest => dest.AdditionalInfo.Active, opt => opt.MapFrom(src => !src.Deleted))
            .ForPath(dest => dest.AdditionalInfo.Amount, opt => opt.MapFrom(src => src.Amount));
    }
}