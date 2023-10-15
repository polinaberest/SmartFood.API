using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class StoredDishesController : ODataControllerBase<StoredDish>
    {
        public StoredDishesController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}