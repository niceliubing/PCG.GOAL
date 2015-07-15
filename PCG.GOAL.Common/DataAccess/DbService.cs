using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCG.GOAL.Common.Interface;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.Common.DataAccess
{
    public class DbService : IDbService
    {
        private readonly SqlDataAccess _sqlDataAccess;

        public DbService()
        {
            _sqlDataAccess = new SqlDataAccess();
        }

        public Credentials GetCredentials(string username)
        {
            // todo: get the hashedPassword from database with _sqlDataAccess
            //var credentialsReader =_sqlDataAccess.GetReader("sp_GetCredentials",
            //    new List<SqlParameter> {new SqlParameter("username", username)});

            var passwordHasher = new PasswordHasher();
            var hashedPassword = passwordHasher.HashPassword(username);
            return new Credentials{Username = username,Password = hashedPassword};
        }

        public ClientInfo GetClientInfo(string clientId)
        {
            // todo: get the hashedClientSecret from database with _sqlDataAccess
            //var clientInfoReader =_sqlDataAccess.GetReader("sp_GetClientInfo",
            //    new List<SqlParameter> {new SqlParameter("clientId", clientId)});

            var passwordHasher = new PasswordHasher();
            var hashedClientSecret = passwordHasher.HashPassword(clientId);
            return new ClientInfo {ClientId = clientId, ClientSecret = hashedClientSecret};
        }
    }
}