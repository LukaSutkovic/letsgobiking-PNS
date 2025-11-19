using ProxyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using ProxyCore.Dto;
namespace Routing
{
    [ServiceContract]
    public interface IRouting
    {
        [OperationContract]
        [WebGet(UriTemplate = "/contracts", ResponseFormat = WebMessageFormat.Json)]
        Task<List<Contract>> Contracts();

        [OperationContract]
        [WebGet(UriTemplate = "/stations?contract={contract}", ResponseFormat = WebMessageFormat.Json)]
        Task<List<Station>> Stations(string contract);
    }
}
