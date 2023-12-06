using AutoMapper;
using Delivery_API.Data;
using Delivery_API.Exceptions;
using Delivery_API.Services.IServices;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Entity;
using Delivery_Models.Models.Enum;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Delivery_API.Services
{
    public class DishService : IDishService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private const int PageSize = 8;

        public DishService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DishPagedListDto> GetDish(DishCategory[] category, DishSorting sorting, bool vegetarian, int page)
        {
            IQueryable<Dish> dishQueryable = _context.Dishes;

            //filter by category
            dishQueryable = !category.IsNullOrEmpty() ? dishQueryable.Where(x => category != null && category.Contains(x.Category)) : dishQueryable;
            //filter by vagetarian
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
            List<DishDto> DishDtos = new();

            foreach (var item in dishes)
            {
                DishDto DishDto = new()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    Image = item.Image,
                    Vegetarian = item.Vegetarian,
                    Rating = item.Rating,
                    Category = item.Category
                };

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
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Dish with id = {id} don't in database"
                });
            }

            return _mapper.Map<DishDto>(dish);
        }

        public async Task<bool> CheckRating(Guid dishId, Guid userId)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish == null)
            {
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Dish with id = {dishId} don't in database"
                });
            }
            foreach (var order in _context.Orders)
            {
                if (order.UserId == userId && order.Status == OrderStatus.Delivered)
                {
                    foreach (var cart in _context.OrderBaskets)
                    {
                        if (cart.OrderId == order.Id && cart.DishId == dishId)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        public async Task SetRating(Guid dishId, int ratingScore, Guid userId)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish == null)
            {
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Dish with id = {dishId} don't in database"
                });
            }
            if (!await CheckRating(dishId, userId))
            {
                throw new BadRequestException(new Response
                {
                    Status = "Error",
                    Message = "User can't set rating on dish that wasn't ordered"
                });
            }

            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.DishId == dishId);
            if (rating != null)
            {
                rating.RatingScore = ratingScore;
                _context.Ratings.Update(rating);
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    Id = Guid.NewGuid(),
                    DishId = dishId,
                    UserId = userId,
                    RatingScore = ratingScore
                });

            }
            await _context.SaveChangesAsync();

            double dishRating = _context.Ratings
            .Where(rating => rating.DishId == dishId)
            .Select(rating => rating.RatingScore)
            .ToList()
            .Average();

            dish.Rating = dishRating;
            _context.Dishes.Update(dish);

            await _context.SaveChangesAsync();
        }

        #region admin features
        public async Task<Dish> CreateDish(DishDto dishDto)
        {
            var dish = new Dish
            {
                Id = Guid.NewGuid(),
                Name = dishDto.Name,
                Description = dishDto.Description,
                Price = dishDto.Price,
                Category = dishDto.Category,
                Vegetarian = dishDto.Vegetarian,
                Image = dishDto.Image,
                Rating = dishDto.Rating,
            };
            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
            return dish;
        }
        public async Task UpdateDish(Guid dishId, DishDto dishDto)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish == null)
            {
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Dish with id = {dishId} don't in database"
                });
            }
            dish.Name = dishDto.Name;
            dish.Description = dishDto.Description;
            dish.Price = dishDto.Price;
            dish.Category = dishDto.Category;
            dish.Vegetarian = dishDto.Vegetarian;
            dish.Image = dishDto.Image;
            dish.Rating = dishDto.Rating;

            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDish(Guid dishId)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish == null)
            {
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Dish with id = {dishId} don't in database"
                });
            }
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
