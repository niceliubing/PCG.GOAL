using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.WebService.Test.WebClient
{
    public class OAuthAccess
    {
        public Credentials GetCredentials()
        {
            // todo: get credentials from db
            var credentials = new Credentials { ClientId = "goalview", ClientSecret = "goalview", Username = "admin", Password = "admin" };
            return credentials;
        }
    }
}