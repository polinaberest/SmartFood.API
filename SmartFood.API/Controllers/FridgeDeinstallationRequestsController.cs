using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class FridgeDeinstallationRequestsController : ODataControllerBase<FridgeDeinstallationRequest>
    {
        public FridgeDeinstallationRequestsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        public override async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var deinstallRequest = await AppDbContext.FridgeDeinstallationRequests.FirstOrDefaultAsync(r => r.Id == key);
            if (deinstallRequest == null)
                return NotFound();

            var fridgeToDeinstall = await AppDbContext.Fridges.FirstOrDefaultAsync(f => f.Id == deinstallRequest.FridgeId);
            if (fridgeToDeinstall == null)
                return NotFound();

            AppDbContext.FridgeDeinstallationRequests.Remove(deinstallRequest);
            AppDbContext.Fridges.Remove(fridgeToDeinstall);

            await AppDbContext.SaveChangesAsync();

            return Ok(deinstallRequest);
        }
    }
}