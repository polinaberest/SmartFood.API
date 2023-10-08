using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class DishesController : ODataControllerBase<Dish>
    {
        public DishesController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}