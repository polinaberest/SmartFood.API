using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class FilialsController : ODataControllerBase<Filial>
    {
        public FilialsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        public override async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var filial = await AppDbContext.Filials.FindAsync(key);

            if (filial == null)
            {
                return NotFound();
            }

            filial.IsDeleted = true;

            // Create Deinstall Request for each fridge in filial
            var fridgesToDeinstall = await AppDbContext.Fridges.Where(f => f.FilialId == key).ToListAsync();

            foreach (var fridge in fridgesToDeinstall)
            {
                FridgesController.MakeFridgeDeinstallationRequest(fridge, AppDbContext);
            }

            await AppDbContext.SaveChangesAsync();

            return Ok(filial);
        }
    }
}