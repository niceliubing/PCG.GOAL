using Microsoft.AspNet.Identity;
using PCG.GOAL.Common.Interface;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.Common.DataAccess
{
    public class OAuthValidator : IOAuthValidator
    {
        private readonly IDbService _dbService;

        public OAuthValidator(IDbService dbService)
        {
            _dbService = dbService;
        }

        public bool ValidateClient(string clientId, string clientSecret)
        {
            var client = _dbService.GetClientInfo(clientId);
            return (client != null && VerifyHashedPassword(client.ClientSecret, clientSecret));

        }
        public bool ValidateUser(string username, string password)
        {
            var credentials = _dbService.GetCredentials(username);
            return (credentials != null && VerifyHashedPassword(credentials.Password, password));
        }

        public bool VerifyHashedPassword(string hashedPassword, string password)
        {
            var passwordHasher = new PasswordHasher();
            return passwordHasher.VerifyHashedPassword(hashedPassword, password)==PasswordVerificationResult.Success;
        }
    }
}
