using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class FridgeInstallationRequestsController : ODataControllerBase<FridgeInstallationRequest>
    {
        public FridgeInstallationRequestsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}