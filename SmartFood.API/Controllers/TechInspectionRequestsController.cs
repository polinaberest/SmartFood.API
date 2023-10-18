using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class TechInspectionRequestsController : ODataControllerBase<TechInspectionRequest>
    {
        public TechInspectionRequestsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        public override Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return base.Delete(key);
        }
    }
}