using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class SuppliersController : ODataControllerBase<Supplier>
    {
        public SuppliersController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        public override async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var supplier = await AppDbContext.Suppliers.FindAsync(key);

            if (supplier == null)
            {
                return NotFound();
            }

            supplier.IsBlocked = !supplier.IsBlocked;

            await AppDbContext.SaveChangesAsync();

            return Ok(supplier);
        }
    }
}