using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class OrdersController : ODataControllerBase<Order>
    {
        public OrdersController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        //тут при смене статуса заказа на approved  - когда забираем заказ - отпрвить запрос на увеличение doorsOpensCount

        // когда делаем заказ - уменьшить кол-во хранимого блюда в холодильнике на кол-во в заказе

        [HttpGet("recommended-orders/{userId}")]
        public IActionResult GetOrderRecommendations(Guid userId)
        {
            var allUserOrders = AppDbContext.Orders.Where(o => o.CustomerId == userId)
                                                    .Include(o => o.Customer)
                                                    .Include(o => o.OrderedDish)
                                                    .ToList();
            if (allUserOrders == null)
            {
                return NotFound();
            }

            var availableOrders = allUserOrders.Where(o => o.Count <= o.OrderedDish.CountAvailable)
                                               .ToList();

            var uniqueOrderedDishes = availableOrders
                                        .GroupBy(o => o.OrderedDish.Id)
                                        .Select(group => group.OrderBy(o => o.TotalPrice).First())
                                        .ToList();

            if (uniqueOrderedDishes.Any())
            {
                return Ok(uniqueOrderedDishes);
            }

            return Ok(new List<Order>());
        }
    }
}