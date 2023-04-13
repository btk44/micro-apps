using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class CategoryGroupMapper: Profile {
    public CategoryGroupMapper()
    {
        CreateMap<CategoryGroupEntity, CategoryGroupDto>();
        CreateMap<CategoryGroupDto, CategoryGroupEntity>();
    }
}