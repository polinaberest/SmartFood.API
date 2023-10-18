using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmartFood.Domain;
using SmartFood.Domain.Models;
using System.Text.Json;

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

            await AppDbContext.SaveChangesAsync();

            return Ok(request);
        }

        private async void InstallFridge(FridgeInstallationRequest request, string uri)
        {
            var fridgeToInstall = new Fridge
            {
                PlacementDescription = request.PlacementDescription,
                IsOpen = false,
                FilialId = request.FilialId,
                URI = uri
            };

            AppDbContext.Fridges.Add(fridgeToInstall);
            await AppDbContext.SaveChangesAsync();
        }


        [HttpPost]
        [Route("fridge-install-requests/{installRequestId}/fulfill")]
        public async Task<IActionResult> FulfillRequest(Guid installRequestId, string uri)
        {
            var fridgeInstallReguest = await AppDbContext.FridgeInstallationRequests.FirstOrDefaultAsync(f => f.Id == installRequestId);

            if (fridgeInstallReguest == null)
            {
                return NotFound();
            }

            fridgeInstallReguest.Status = RequestStatus.Fulfilled;
            fridgeInstallReguest.AnsweredTime = DateTime.UtcNow;

            InstallFridge(fridgeInstallReguest, uri);
            return Ok(fridgeInstallReguest);
        }

        [HttpGet("fridge-install-requests/checkUri")]
        public ActionResult<bool> CheckUrl(string uri)
        {
            bool uriExists = CheckIfUriExists(uri);
            return Ok(uriExists);
        }

        private bool CheckIfUriExists(string uri)
        {
            return AppDbContext.Fridges.Any(f => f.URI == uri);
        }

    }
}