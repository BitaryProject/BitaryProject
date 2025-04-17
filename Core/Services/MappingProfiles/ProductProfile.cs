using Core.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Shared.ProductModels;

namespace Services.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductBrand, BrandResultDTO>();
            CreateMap<ProductCategory, CategoryResultDTO>();


            CreateMap<Product, ProductResultDTO>()
                .ForMember(d => d.BrandName, options => options.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.CategoryName, options => options.MapFrom(s => s.ProductCategory.Name));
        }
    }
}
