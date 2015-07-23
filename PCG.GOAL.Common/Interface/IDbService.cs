using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.Common.Interface
{
    public interface IDbService
    {
        Credentials GetCredentials(string username);
        Credentials GetCredentialsById(int id);
        IEnumerable<Credentials> GetAllCredentials();
        void AddCredentials(Credentials credentials);
        void UpdateCredentials(Credentials credentials);
        void DeleteCredentials(int id);


        ClientInfo GetClientInfo(string clientId);
        ClientInfo GetClientInfoById(int id);
        IEnumerable<ClientInfo> GetAllClientInfo();
        void AddClientInfo(ClientInfo clientInfo);
        void UpdateClientInfo(ClientInfo clientInfo);
        void DeleteClientInfo(int id);
    }
}
