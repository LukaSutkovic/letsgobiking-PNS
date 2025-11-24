using System.ServiceModel; // Le standard SOAP
using System.Threading.Tasks;

namespace RoutingService.Services
{
    [ServiceContract] // Définit ceci comme un service SOAP
    public interface IRoutingSoapService
    {
        [OperationContract] // Cette méthode sera visible dans le WSDL
        Task<RouteLeg> GetItineraryAsync(string depart, string arrivee);
    }
}