using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class OrdersController : ODataControllerBase<Order>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public OrdersController(ApplicationDbContext appDbContext, IHttpClientFactory httpClientFactory) : base(appDbContext)
        {
            this.httpClientFactory = httpClientFactory;
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

        public async override Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Order entity)
        {
            var order = AppDbContext.Orders.Include(o => o.OrderedDish).FirstOrDefault(o => o.Id == key);
            if (order == null)
            {
                return NotFound();
            }

            var fridge = AppDbContext.Fridges.FirstOrDefault(f => f.Id == order.OrderedDish.FridgeId);

            await UpdateFridgeDoorOpenCount(fridge.URI);
            await UpdateFridgeStatistics(fridge.URI);

            return await base.Put(key, entity);
        }

        [HttpGet("fridge/{fridgeId}/orders")]
        public async Task<IActionResult> GetFridgeOrders(Guid fridgeId)
        {
            Order[] result = await AppDbContext.Orders.Include(c => c.OrderedDish)
                .Where(o => o.OrderedDish.FridgeId == fridgeId).ToArrayAsync();

            if (result == null || !result.Any())
            {
                return NotFound("No orders found for the specified fridgeId.");
            }

            return Ok(result);
        }

        private async Task UpdateFridgeDoorOpenCount(string uri)
        {
            var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            var response = await client.PutAsJsonAsync(new Uri(uri + "/TechInspection/updateDoorOpenCount").ToString(), new { });
        }

        private async Task UpdateFridgeStatistics(string uri)
        {
            var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            var response = await client.PutAsJsonAsync(new Uri(uri + "/Statistics/updateStatistics").ToString(), new { });
        }
    }
}