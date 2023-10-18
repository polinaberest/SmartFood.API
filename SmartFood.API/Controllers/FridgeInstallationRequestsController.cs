using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class FridgeInstallationRequestsController : ODataControllerBase<FridgeInstallationRequest>
    {
        public FridgeInstallationRequestsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        public override async Task<IActionResult> Put([FromODataUri] Guid key, [FromBody] FridgeInstallationRequest entity)
        {
            var request = await AppDbContext.FridgeInstallationRequests.FirstOrDefaultAsync(f => f.Id == key);

            if (request == null)
                return NotFound();

            request.Status = entity.Status;
            request.AnsweredTime = DateTime.UtcNow;

            if(request.Status == RequestStatus.Fulfilled)
                InstallFridge(request);

            await AppDbContext.SaveChangesAsync();

            return Ok(request);
        }

        private async void InstallFridge(FridgeInstallationRequest request)
        {
            var fridgeToInstall = new Fridge
            {
                PlacementDescription = request.PlacementDescription,
                IsOpen = false,
                FilialId = request.FilialId
            };

            AppDbContext.Fridges.Add(fridgeToInstall);
            await AppDbContext.SaveChangesAsync();
        }
    }
}