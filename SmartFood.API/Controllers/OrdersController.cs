using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class OrdersController : ODataControllerBase<Order>
    {
        public OrdersController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}