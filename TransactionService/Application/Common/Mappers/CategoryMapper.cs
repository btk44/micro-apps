using AutoMapper;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Common.Mappers;

public class CategoryMapper: Profile {
    public CategoryMapper()
    {
        CreateMap<CategoryEntity, CategoryDto>()
            .ForPath(dest => dest.Deleted, opt => opt.MapFrom(src => !src.Active));
        CreateMap<CategoryDto, CategoryEntity>()
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => !src.Deleted));

    }
}