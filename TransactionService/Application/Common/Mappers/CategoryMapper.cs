using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class CategoryMapper: Profile {
    public CategoryMapper()
    {
        CreateMap<CategoryEntity, CategoryDto>();
        CreateMap<CategoryDto, CategoryEntity>();
    }
}