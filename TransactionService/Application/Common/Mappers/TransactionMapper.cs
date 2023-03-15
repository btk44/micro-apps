using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class TransactionMapper: Profile {
    public TransactionMapper()
    {
        CreateMap<TransactionEntity, TransactionDto>()
            .ForMember(dest => dest.Deleted, opt => opt.MapFrom(src => !src.Active))
            .ForPath(dest => dest.Payee, opt => opt.MapFrom(src => src.AdditionalInfo.Payee))
            .ForPath(dest => dest.Comment, opt => opt.MapFrom(src => src.AdditionalInfo.Comment));
        CreateMap<TransactionDto, TransactionEntity>()
            .ForMember(dest => dest.Account, opt => opt.MapFrom(src => !src.Deleted))
            .ForPath(dest => dest.AdditionalInfo.Payee, opt => opt.MapFrom(src => src.Payee))
            .ForPath(dest => dest.AdditionalInfo.Comment, opt => opt.MapFrom(src => src.Comment));
    }
}