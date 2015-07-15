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
        ClientInfo GetClientInfo(string clientId);
    }
}
