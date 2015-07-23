using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.Common.Interface
{
    public interface IOAuthValidator
    {
        bool ValidateClient(string clientId, string clientSecret);
        Credentials ValidateUser(string username, string password);
        bool VerifyHashedPassword(string hashedPassword, string password);
    }
}
