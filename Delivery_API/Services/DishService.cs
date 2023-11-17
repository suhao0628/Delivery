using AutoMapper;
using Delivery_API.Data;
using Delivery_API.Models;
using Delivery_API.Models.Dto;
using Delivery_API.Models.Entity;
using Delivery_API.Models.Enum;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Delivery_API.Services
{
    public class DishService : IDishService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private const int PageSize = 5;

        public DishService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DishPagedListDto> GetDish(DishCategory[] category, DishSorting sorting, bool vegetarian, int page)
        {

            IQueryable<Dish> dishQueryable = _context.Dishes;

            //filter by category
            //if(!category.IsNullOrEmpty() )
            //{
            //    dishQueryable = dishQueryable.Where(x => category.Contains(x.Category));
            //}

            dishQueryable = !category.IsNullOrEmpty() ? dishQueryable.Where(x => category != null && category.Contains(x.Category)) : dishQueryable;
            //filter by vagetarian
            //if (vegetarian)
            //{
            //    dishQueryable = dishQueryable.Where(x => x.Vegetarian);
            //}
            dishQueryable = vegetarian ? dishQueryable.Where(x => x.Vegetarian) : dishQueryable;

            //filter by sort
            dishQueryable = Sort(dishQueryable, sorting);

            //pagination
            int dishCount = await dishQueryable.CountAsync();
            int pageTotal = (int)Math.Ceiling(dishCount / (double)PageSize);

            if (page < 1)
            {
                page = 1;
            }
            if (pageTotal == 0)
            {
                pageTotal = 1;
                page = 1;
            }
            else
            {
                if (page > pageTotal)
                {
                    page = pageTotal;
                }
            }
            var dishes = dishQueryable.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            PageInfoModel paginationModel = new()
            {
                Size = PageSize,
                Count = pageTotal,
                Current = page
            };
            List<DishDto> DishDtos = new List<DishDto>();

            foreach (var item in dishes)
            {
                DishDto DishDto = new DishDto();
                DishDto.Id = item.Id;
                DishDto.Name = item.Name;
                DishDto.Description = item.Description;
                DishDto.Price = item.Price;
                DishDto.Image = item.Image;
                DishDto.Vegetarian = item.Vegetarian;
                DishDto.Rating = item.Rating;
                DishDto.Category = item.Category;

                DishDtos.Add(DishDto);
            }
            DishPagedListDto dishListDto = new()
            {
                Dishes = DishDtos,
                Pagination = paginationModel
            };
            return dishListDto;
        }

        private static IQueryable<Dish> Sort(IQueryable<Dish> dishes, DishSorting? sorting = null)
        {
            return sorting switch
            {
                DishSorting.NameAsc => dishes.OrderBy(d => d.Name),
                DishSorting.NameDesc => dishes.OrderByDescending(d => d.Name),
                DishSorting.PriceAsc => dishes.OrderBy(d => d.Price),
                DishSorting.PriceDesc => dishes.OrderByDescending(d => d.Price),
                DishSorting.RatingAsc => dishes.OrderBy(d => d.Rating),
                DishSorting.RatingDesc => dishes.OrderByDescending(d => d.Rating),
                _ => throw new Exception()
            };

        }

        public async Task<DishDto> GetDishDetails(Guid id)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null)
            {
                throw new Exception("dish not found");
            }
            
            return _mapper.Map<DishDto>(dish);
        }
    }
}
