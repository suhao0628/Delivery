using AutoMapper;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Entity;
using System.Collections.Generic;

namespace Delivery_API
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserRegisterModel>().ReverseMap();

            CreateMap<Dish, DishDto>().ReverseMap();
            CreateMap<Basket, DishBasketDto>().ReverseMap();

            CreateMap<Order, OrderCreateDto>().ReverseMap();
           // CreateMap<List<OrderInfoDto>, List<Order>>().ReverseMap();
            //CreateMap<List<DishBasketDto>, List <OrderBasket>>().ReverseMap();
            


        }
    }
}
