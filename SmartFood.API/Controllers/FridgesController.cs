using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class FridgesController : ODataControllerBase<Fridge>
    {
        public FridgesController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        public override async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var fridgeToDeinstall = await AppDbContext.Fridges.FirstOrDefaultAsync(f => f.Id == key);

            if (fridgeToDeinstall == null)
                return NotFound();

            MakeFridgeDeinstallationRequest(fridgeToDeinstall, AppDbContext);

            await AppDbContext.SaveChangesAsync();

            return Ok(fridgeToDeinstall);
        }

        public static void MakeFridgeDeinstallationRequest(Fridge fridge, ApplicationDbContext applicationDbContext)
        {
            fridge.IsDeleted = true;

            // TODO : remove dishes from fridge - delete stored dishes + requests to store dishes in fridge
            RemoveStoredDishes(fridge.Id, applicationDbContext);
            RemoveStorageRequests(fridge.Id, applicationDbContext);

            var deinstallRequest = new FridgeDeinstallationRequest
            {
                FridgeId = fridge.Id,
                Fridge = fridge
            };

            applicationDbContext.FridgeDeinstallationRequests.Add(deinstallRequest);
        }

        private static void RemoveStoredDishes(Guid fridgeId, ApplicationDbContext applicationDbContext) 
        {
            var dishesToRemove = applicationDbContext.StoredDishes.Include(d => d.Dish).Where(d => d.FridgeId == fridgeId).ToList();

            foreach (var dish in dishesToRemove)
            {
                applicationDbContext.StoredDishes.Remove(dish);
            }
        }

        private static void RemoveStorageRequests(Guid fridgeId, ApplicationDbContext applicationDbContext)
        {
            var requestsToRemove = applicationDbContext.FridgeUsageRequests.Where(r => r.FridgeId == fridgeId).ToList();

            foreach (var request in requestsToRemove)
            {
                applicationDbContext.FridgeUsageRequests.Remove(request);
            }
        }
    }
}