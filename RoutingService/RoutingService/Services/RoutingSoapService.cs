using System.Threading.Tasks;

namespace RoutingService.Services
{
    public class RoutingSoapService : IRoutingSoapService
    {
        // On réutilise ton service existant !
        private readonly ItineraryService _itineraryService = new ItineraryService();

        public async Task<RouteLeg> GetItineraryAsync(string depart, string arrivee)
        {
            // On appelle la même logique que le REST
            return await _itineraryService.CalculateItineraryAsync(depart, arrivee);
        }
    }
}