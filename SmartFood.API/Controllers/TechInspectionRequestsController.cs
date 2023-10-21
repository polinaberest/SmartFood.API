using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain;
using SmartFood.Domain.Models;

namespace SmartFood.API.Controllers
{
    public class TechInspectionRequestsController : ODataControllerBase<TechInspectionRequest>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public TechInspectionRequestsController(ApplicationDbContext appDbContext, 
            IHttpClientFactory httpClientFactory) : base(appDbContext)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async override Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var techInspectionRequest = await AppDbContext.TechInspectionRequests.FindAsync(key);
            if (techInspectionRequest == null) 
            {
                return NotFound();
            }

            var fridge = await AppDbContext.Fridges.FirstOrDefaultAsync(f => f.Id == techInspectionRequest.FridgeId);

            if (fridge != null)
                await ResetFridgeValues(fridge.URI);            

            return await base.Delete(key);
        }

        private async Task ResetFridgeValues(string uri)
        {
            var client = httpClientFactory.CreateClient();
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Content = null
            };
            var response = await client.SendAsync(request);
        }

    }
}