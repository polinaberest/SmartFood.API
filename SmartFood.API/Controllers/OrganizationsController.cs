using SmartFood.Domain;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class OrganizationsController : ODataControllerBase<Organization>
    {
        public OrganizationsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}