using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class SuppliersController : ODataControllerBase<Supplier>
    {
        public SuppliersController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}