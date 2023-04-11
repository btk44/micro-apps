using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class TransactionMapper: Profile {
    public TransactionMapper()
    {
        CreateMap<TransactionEntity, TransactionDto>();
        CreateMap<TransactionDto, TransactionEntity>();
    }
}