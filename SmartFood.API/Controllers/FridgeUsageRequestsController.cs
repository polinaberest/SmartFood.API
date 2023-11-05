using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var fridgeUsageReguest = await AppDbContext.FridgeUsageRequests.Include(c => c.Dish).Include(c => c.Fridge)
                .FirstOrDefaultAsync(c => c.Id == usageRequestId);

            if (fridgeUsageReguest == null)
            {
                return NotFound();
            }

            fridgeUsageReguest.Status = RequestStatus.Approved;
            fridgeUsageReguest.AnsweredTime = DateTime.UtcNow;

            //var dish = await AppDbContext.Dishes.AsNoTracking().FindAsync(fridgeUsageReguest.DishId);
            //var fridge = await AppDbContext.Fridges.AsNoTracking().FindAsync(fridgeUsageReguest.FridgeId);

            // Create StoredDish
            var storedDish = new StoredDish
            {
                CountAvailable = 0,
                DishId = fridgeUsageReguest.DishId,
                Dish = fridgeUsageReguest.Dish,
                FridgeId = fridgeUsageReguest.FridgeId,
                Fridge = fridgeUsageReguest.Fridge,
            };

            await AppDbContext.StoredDishes.AddAsync(storedDish);
            await AppDbContext.SaveChangesAsync();

            return Ok();
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