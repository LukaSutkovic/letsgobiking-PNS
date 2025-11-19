using Microsoft.AspNetCore.Mvc;
using RoutingService.Services;
using System.Threading.Tasks;

namespace RoutingService.Controllers
{
    [Route("api/routing")]
    [ApiController]
    public class RoutingController : ControllerBase
    {
        private readonly ItineraryService _itineraryService = new ItineraryService();

        [HttpGet]
        public async Task<IActionResult> GetItinerary([FromQuery] string depart, [FromQuery] string arrivee)
        {
            
            RouteLeg result = await _itineraryService.CalculateItineraryAsync(depart, arrivee);

            
            return Ok(result);
        }
    }
}