using Microsoft.AspNetCore.Mvc;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class FridgeUsageRequestsController : ODataControllerBase<FridgeUsageRequest>
    {
        public FridgeUsageRequestsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }


        [HttpPut]
        [Route("fridge-use-requests/{usageRequestId}/accept")]
        public async Task<IActionResult> AcceptRequest(Guid usageRequestId)
        {
            var fridgeUsageReguest = await AppDbContext.FridgeUsageRequests.FindAsync(usageRequestId);

            if (fridgeUsageReguest == null)
            {
                return NotFound();
            }

            fridgeUsageReguest.Status = RequestStatus.Approved;
            fridgeUsageReguest.AnsweredTime = DateTime.UtcNow;

            var dish = await AppDbContext.Dishes.FindAsync(fridgeUsageReguest.DishId);
            var fridge = await AppDbContext.Fridges.FindAsync(fridgeUsageReguest.FridgeId);

            // Create StoredDish
            var storedDish = new StoredDish
            {
                CountAvailable = 0,
                DishId = fridgeUsageReguest.DishId,
                Dish = dish,
                FridgeId = fridgeUsageReguest.FridgeId,
                Fridge = fridge,
            };

            await AppDbContext.StoredDishes.AddAsync(storedDish);
            await AppDbContext.SaveChangesAsync();

            return Ok(fridgeUsageReguest);
        }

        [HttpPut]
        [Route("fridge-use-requests/{usageRequestId}/reject")]
        public async Task<IActionResult> RejectRequest(Guid usageRequestId)
        {
            var fridgeUsageReguest = await AppDbContext.FridgeUsageRequests.FindAsync(usageRequestId);

            if (fridgeUsageReguest == null)
            {
                return NotFound();
            }

            fridgeUsageReguest.Status = RequestStatus.Rejected;
            fridgeUsageReguest.AnsweredTime = DateTime.UtcNow;
            await AppDbContext.SaveChangesAsync();

            return Ok(fridgeUsageReguest);
        }
    }
}