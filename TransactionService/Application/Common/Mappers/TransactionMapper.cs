using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class TransactionMapper: Profile {
    public TransactionMapper()
    {
        CreateMap<TransactionEntity, TransactionDto>()
            .ForPath(dest => dest.Payee, opt => opt.MapFrom(src => src.AdditionalInfo.Payee))
            .ForPath(dest => dest.Comment, opt => opt.MapFrom(src => src.AdditionalInfo.Comment));
        CreateMap<TransactionDto, TransactionEntity>()
            .ForPath(dest => dest.AdditionalInfo.Payee, opt => opt.MapFrom(src => src.Payee))
            .ForPath(dest => dest.AdditionalInfo.Comment, opt => opt.MapFrom(src => src.Comment));
    }
}