using AutoMapper;
using ProductShop.DataTransferObjects;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {

            CreateMap<UserInputModel, User>();
            CreateMap<ProductInputModel, Product>();
            CreateMap<CategoriesInputModel, Category>();
            CreateMap<CategoriesProductsInputModel, CategoryProduct>();

        }
    }
}
