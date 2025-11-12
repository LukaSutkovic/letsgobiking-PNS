using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using ProxyCore.Dto;

namespace ProxyCore
{
    [ServiceContract]
    public interface IProxy
    {
        [OperationContract]
        [WebGet(UriTemplate = "/contracts", ResponseFormat = WebMessageFormat.Json)]
        Task<List<Contract>> GetContracts();

        [OperationContract]
        [WebGet(UriTemplate = "/stations?contract={contract}", ResponseFormat = WebMessageFormat.Json)]
        Task<List<Station>> GetStations(string contract);
    }
}
