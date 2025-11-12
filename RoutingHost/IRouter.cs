using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace RoutingHost
{
    [ServiceContract]
    public interface IRouter
    {
        // /api/route?origin=lat,lon&dest=lat,lon
        [OperationContract]
        [WebGet(UriTemplate = "/route?origin={origin}&dest={dest}", ResponseFormat = WebMessageFormat.Json)]
        Task<RouteResponse> GetRoute(string origin, string dest);
    }
}
