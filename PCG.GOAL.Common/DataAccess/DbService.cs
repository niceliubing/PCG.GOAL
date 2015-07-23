using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PCG.GOAL.Common.Interface;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.Common.DataAccess
{
    public class DbService : IDbService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private const string GoalInternal = "goal_internal";

        #region Constructor
        public DbService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        #endregion

        #region Credentials



        public IEnumerable<Credentials> GetAllCredentials()
        {

            var sqlStatement = string.Format("SELECT * FROM ServiceUser");
            try
            {
                using (var credentialsTable = _sqlDataAccess.GetTableBySql(sqlStatement))
                {
                    var data = credentialsTable.AsEnumerable().Select(r => new Credentials
                        {
                            Id = (int)r["id"],
                            Username = (string)r["username"],
                            Password = (string)r["password"],
                            Role = r["role"] == DBNull.Value ? string.Empty : (string)r["role"]
                        });
                    return data.Where(c => c.Username != GoalInternal).ToList();
                }
            }
            catch (Exception e)
            {
                // log error
                throw;
            }

        }

        public Credentials GetCredentialsById(int id)
        {
            var sqlStatement = string.Format("SELECT * FROM ServiceUser WHERE id = '{0}'", id);
            using (var credentialsTable = _sqlDataAccess.GetTableBySql(sqlStatement))
            {
                var data = credentialsTable.AsEnumerable().Select(r => new Credentials
                {
                    Id = (int)r["id"],
                    Username = (string)r["username"],
                    Password = (string)r["password"],
                    Role = r["role"] == DBNull.Value ? string.Empty : (string)r["role"]
                }).FirstOrDefault();
                return data;
            }
        }

        public Credentials GetCredentials(string username)
        {
            var sqlStatement = string.Format("SELECT * FROM ServiceUser WHERE USERNAME= '{0}'", username);
            using (var credentialsTable = _sqlDataAccess.GetTableBySql(sqlStatement))
            {
                var data = credentialsTable.AsEnumerable().Select(r => new Credentials
                {
                    Id = (int)r["id"],
                    Username = (string)r["username"],
                    Password = (string)r["password"],
                    Role = r["role"] == DBNull.Value ? string.Empty : (string)r["role"]
                }).FirstOrDefault();
                return data;
            }
        }

        public void AddCredentials(Credentials credentials)
        {
            var sqlStatement = string.Format("INSERT INTO ServiceUser (username, password, role) VALUES ('{0}','{1}','{2}')",
                credentials.Username,credentials.Password,credentials.Role);
            try
            {
                _sqlDataAccess.ExecuteNonQuery(sqlStatement);
            }
            catch (Exception e)
            {
                // log error
                throw;
            }
        }

        public void UpdateCredentials(Credentials credentials)
        {
            var sqlStatement = string.Format("UPDATE ServiceUser SET username = '{0}', password = '{1}', role = '{2}' WHERE id = {3} ",
                credentials.Username, credentials.Password, credentials.Role,credentials.Id);
            try
            {
                _sqlDataAccess.ExecuteNonQuery(sqlStatement);
            }
            catch (Exception e)
            {
                // log error
                throw;
            }
        }

        public void DeleteCredentials(int id)
        {
            var sqlStatement = string.Format("DELETE ServiceUser  WHERE id = '{0}'", id);
            try
            {
                _sqlDataAccess.ExecuteNonQuery(sqlStatement);
            }
            catch (Exception e)
            {
                // log error
                throw;
            }
        }

        #endregion

        #region ClientInfo


        public IEnumerable<ClientInfo> GetAllClientInfo()
        {
            var sqlStatement = string.Format("SELECT * FROM ServiceClient");
            try
            {
                using (var credentialsTable = _sqlDataAccess.GetTableBySql(sqlStatement))
                {
                    var data = credentialsTable.AsEnumerable().Select(r => new ClientInfo
                    {
                        Id = (int)r["id"],
                        ClientId = (string)r["clientid"],
                        ClientSecret = (string)r["clientsecret"],
                        Description = r["Description"] == DBNull.Value ? string.Empty : (string)r["Description"]
                    });
                    return data.Where(c => c.ClientId != GoalInternal).ToList();
                }
            }
            catch (Exception e)
            {
                // log error
                throw;
            }
        }

        public ClientInfo GetClientInfoById(int id)
        {
            var sqlStatement = string.Format("SELECT * FROM ServiceClient WHERE id = '{0}'", id);
            using (var credentialsTable = _sqlDataAccess.GetTableBySql(sqlStatement))
            {
                var data = credentialsTable.AsEnumerable().Select(r => new ClientInfo
                {
                    Id = (int)r["id"],
                    ClientId = (string)r["clientid"],
                    ClientSecret = (string)r["clientsecret"],
                    Description = r["Description"] == DBNull.Value ? string.Empty : (string)r["Description"]
                }).FirstOrDefault();
                return data;
            }
        }
        
        public ClientInfo GetClientInfo(string clientId)
        {
            var sqlStatement = string.Format("SELECT * FROM ServiceClient WHERE CLIENTID= '{0}'", clientId);
            using (var credentialsTable = _sqlDataAccess.GetTableBySql(sqlStatement))
            {
                var data = credentialsTable.AsEnumerable().Select(r => new ClientInfo
                {
                    Id = (int)r["id"],
                    ClientId = (string)r["clientid"],
                    ClientSecret = (string)r["clientsecret"],
                    Description = r["Description"] == DBNull.Value ? string.Empty : (string)r["Description"]
                }).FirstOrDefault();
                return data;
            }

        }

        public void AddClientInfo(ClientInfo clientInfo)
        {
            var sqlStatement = string.Format("INSERT INTO ServiceClient (clientid, clientsecret, description) VALUES ('{0}','{1}','{2}')",
                clientInfo.ClientId, clientInfo.ClientSecret,clientInfo.Description);
            try
            {
                _sqlDataAccess.ExecuteNonQuery(sqlStatement);
            }
            catch (Exception e)
            {
                // log error
                throw;
            }
        }

        public void UpdateClientInfo(ClientInfo clientInfo)
        {
            var sqlStatement = string.Format("UPDATE ServiceClient SET clientid = '{0}', clientsecret = '{1}', description = '{2}' WHERE id = {3} ",
               clientInfo.ClientId, clientInfo.ClientSecret,clientInfo.Description,clientInfo.Id);
            try
            {
                _sqlDataAccess.ExecuteNonQuery(sqlStatement);
            }
            catch (Exception e)
            {
                // log error
                throw;
            }
        }

        public void DeleteClientInfo(int id)
        {
            var sqlStatement = string.Format("DELETE ServiceClient  WHERE id = '{0}'", id);
            try
            {
                _sqlDataAccess.ExecuteNonQuery(sqlStatement);
            }
            catch (Exception e)
            {
                // log error
                throw;
            }
        }

        #endregion
    }
}