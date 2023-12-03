using Delivery_API.Data;
using Delivery_API.Exceptions;
using Delivery_API.Services.IServices;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Delivery_API.Services
{
    public class BasketService : IBasketService
    {
        private readonly AppDbContext _context;
        public BasketService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<DishBasketDto>> GetBasket(Guid userId)
        {
            var baskets = await _context.Baskets.Where(b => b.UserId == userId).Include(w => w.Dish).ToListAsync();

            List<DishBasketDto> basketsDtos = new List<DishBasketDto>();
            foreach (var item in baskets)
            {
                DishBasketDto basketsDto = new()
                {
                    Id = item.DishId,
                    Name = item.Dish.Name,
                    Price = item.Dish.Price,
                    TotalPrice = item.Dish.Price * item.Amount,
                    Amount = item.Amount,
                    Image = item.Dish.Image
                };
                basketsDtos.Add(basketsDto);
            }
            return basketsDtos;
        }

        public async Task AddBasket(Guid dishId, Guid userId)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);

            if (dish == null)
            {
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Dish with id = {dishId} don't in basket"
                });
            }

            var baskets = await _context.Baskets.FirstOrDefaultAsync(x => x.UserId == userId && x.DishId == dishId);
            if (baskets == null)
            {
                Basket basket = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DishId = dish.Id
                };
                basket.Amount += 1;

                await _context.AddAsync(basket);
                await _context.SaveChangesAsync();
            }
            else
            {
                baskets.Amount += 1;
                _context.Update(baskets);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBasket(Guid dishId, Guid userId, bool increase)
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
            var baskets =
                await _context.Baskets.FirstOrDefaultAsync(x => x.UserId == userId && x.DishId == dishId);

            if (baskets == null)
            {
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Dish with id = {dishId} don't in basket"
                });
            }

            if (baskets.Amount == 1 || !increase)
            {
                _context.Baskets.Remove(baskets);
                await _context.SaveChangesAsync();
            }
            else
            {
                baskets.Amount -= 1;
                _context.Update(baskets);
                await _context.SaveChangesAsync();
            }
        }
    }

}
