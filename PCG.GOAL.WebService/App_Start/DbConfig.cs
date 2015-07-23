using System;
using System.Security.Policy;
using PCG.GOAL.Common.DataAccess;
using PCG.GOAL.Common.WebModels;
using PCG.GOAL.ExternalDataService.Service;

namespace PCG.GOAL.WebService
{
    public class DbConfig
    {
        private const string HashInternal = "ADBx61Nqj+eqoVtXgTsp9bQ/UMZUpUovP0CLPL6rLYWavrE5pndozRPzhy/vDtGlKQ==";
        private const string HashAdmin = "AG7Bxc/IpGq5SXqQDIN26kNbr4Y+jgw9GhfQ26XUls0nYWgoclLDslM9IDKTenBQ5w==";
        private const string HashGoalView = "AFuLeuok3BFeogvkeFZwHAZEDbq5D/5UUaaladYFceKILn2QoWAZ6/VSk1Wjf6kFrw==";
        private const string RoleAdmin = "Admin";
        private const string GoalInternal = "goal_internal";

        public static void Seeding()
        {
            try
            {
                // goal_internal
                var dbService = new DbService(new SqlDataAccess());
                var user = dbService.GetCredentials(GoalInternal);
                if (user != null)
                {
                    user.Password = HashInternal;
                    user.Role = "Admin";
                    dbService.UpdateCredentials(user);
                }
                else
                {
                    dbService.AddCredentials(new Credentials { Username = GoalInternal, Password = HashInternal, Role = RoleAdmin });
                }
                // admin
                user = dbService.GetCredentials("admin");
                if (user != null && user.Role != RoleAdmin)
                {
                    user.Role = RoleAdmin;
                    dbService.UpdateCredentials(user);
                }
                else if(user==null)
                {
                    dbService.AddCredentials(new Credentials { Username = "admin", Password = HashAdmin, Role = RoleAdmin });
                }


                // goal_internal
                var client = dbService.GetClientInfo(GoalInternal);
                if (client != null)
                {
                    client.ClientSecret = HashInternal;
                    dbService.UpdateClientInfo(client);
                }
                else
                {
                    dbService.AddClientInfo(new ClientInfo { ClientId = GoalInternal, ClientSecret = HashInternal });
                }
                
                // goal_internal
                client = dbService.GetClientInfo("goalview");
                if (client == null)
                {
                    dbService.AddClientInfo(new ClientInfo { ClientId = "goalview", ClientSecret = HashGoalView });
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public static string GetInternalToken(string baseUri)
        {
            var client = new WebServiceClient<string>
            {
                Credentials = new Credentials
                {
                    ClientId = GoalInternal,
                    ClientSecret = GoalInternal,
                    Username = GoalInternal,
                    Password = GoalInternal
                },
                BaseUri = baseUri,
                TokenEndpoint = "token"
            };

            return client.GetToken();
        }

    }
}