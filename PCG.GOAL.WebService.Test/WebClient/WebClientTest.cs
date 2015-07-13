using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebAccess;

namespace PCG.GOAL.WebService.Test.WebClient
{
    [TestClass]
    public class WebClientTest
    {
        const string BaseUrl = "http://goalservice.azurewebsites.net";
        const string TokenUrl = BaseUrl + "/token";
        const string EndpointAllStudents = "/api/rethink/Student";
        const string EndpointStudentByIdentity = "/api/rethink/StudentByIdentity?firstname=Brooklin&lastname=Altis&dob=20080605";
        const string EndpointStudentByStatetestnumber = "/api/rethink/StudentByStatetestnumber?statetestnumber=2724167171";

        private Credentials _credentials;
        private Token _token;


        [TestInitialize]
        public void TestInitilaize()
        {
            var oauthAccess = new OAuthAccess();
            _credentials = oauthAccess.GetCredentials();
            _token = GetToken();
        }

        [TestMethod]
        public void CanGetStudentByIdentity()
        {
            try
            {
                var responseData = GetStudentByIdentity();

                Assert.IsNotNull(responseData);
                Assert.IsTrue(responseData.Done);
                Assert.IsInstanceOfType(responseData.Data, typeof(IEnumerable<ChildInfo>));

            }
            catch (Exception ex)
            {
                Assert.IsNull(ex);
            }
        }
        [TestMethod]
        public void CanGetStudentByStatetestnumber()
        {
            try
            {
                var responseData = GetStudentByStatetestnumber();

                Assert.IsNotNull(responseData);
                Assert.IsTrue(responseData.Done);
                Assert.IsInstanceOfType(responseData.Data, typeof(IEnumerable<ChildInfo>));
                Assert.IsTrue(responseData.Data.Count == 1);

            }
            catch (Exception ex)
            {
                Assert.IsNull(ex);
            }
        }

        [TestMethod]
        public void CanGetAllStudents()
        {
            try
            {
                var responseData = GetAllStudents();

                Assert.IsNotNull(responseData);
                Assert.IsTrue(responseData.Done);
                Assert.IsInstanceOfType(responseData.Data, typeof(IEnumerable<ChildInfo>));

            }
            catch (Exception ex)
            {
                Assert.IsNull(ex);
            }
        }
        [TestMethod]
        public void CanGetToken()
        {
            try
            {
                var token = GetToken();
                Assert.IsNotNull(token);
                Assert.IsInstanceOfType(token, typeof(Token));
            }
            catch (Exception ex)
            {
                Assert.IsNull(ex);
            }

        }
        [TestMethod]
        public void CanRefreshToken()
        {
            try
            {
                var token = RefreshToken();
                Assert.IsNotNull(token);
                Assert.IsInstanceOfType(token, typeof(Token));
            }
            catch (Exception ex)
            {
                Assert.IsNull(ex);
            }
        }

        public ResponseData<ChildInfo> GetAllStudents()
        {
            return GetStudentsByEndpoint(BaseUrl + EndpointAllStudents);
        }
        public ResponseData<ChildInfo> GetStudentByIdentity()
        {
            return GetStudentsByEndpoint(BaseUrl + EndpointStudentByIdentity);
        }

        public ResponseData<ChildInfo> GetStudentByStatetestnumber()
        {
            return GetStudentsByEndpoint(BaseUrl + EndpointStudentByStatetestnumber);
        }
        public Token GetToken()
        {
            // web client
            var client = new System.Net.WebClient();


            // invoke the REST method
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _credentials.ClientId, _credentials.ClientSecret)));


            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            var postVaules = new NameValueCollection
            {
                {"username",_credentials.Username},
                {"password", _credentials.Password},
                {"grant_type", GrantTpype.Password}
            };
            try
            {
                byte[] result = client.UploadValues(TokenUrl, "POST", postVaules);
                var jsonData = Encoding.UTF8.GetString(result);
                var token = JsonConvert.DeserializeObject<Token>(jsonData);
                return token;
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new WebException("Failed to request access token. Check you OAuth credentials.");
                }
            }
            return null;
        }
        public Token RefreshToken(Token token = null)
        {
            if (token == null)
            {
                token = GetToken();
            }
            if (token == null || string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                return null;
            }
            // web client
            var client = new System.Net.WebClient();


            // add headers
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _credentials.ClientId, _credentials.ClientSecret)));
            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

            // refresh token request doesn't need "username" and "password"
            var postVaules = new NameValueCollection
            {
                {"grant_type", GrantTpype.RefreshToken},
                {"refresh_token", token.RefreshToken}
            };
            try
            {
                byte[] result = client.UploadValues(TokenUrl, "POST", postVaules);
                var jsonData = Encoding.UTF8.GetString(result);
                var refreshToken = JsonConvert.DeserializeObject<Token>(jsonData);
                return refreshToken;
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new WebException("Failed to request access token");
                }
            }
            return null;
        }

        private ResponseData<ChildInfo> GetStudentsByEndpoint(string endpoint)
        {
            // web client
            var client = new System.Net.WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Headers[HttpRequestHeader.Authorization] = string.Format("Bearer {0}", _token.AccessToken);

            try
            {
                return DownloadStudents(endpoint, client);
            }
            catch (WebException wex)
            {
                if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // validate token failed, need to refresh token
                    _token = RefreshToken(_token);
                    client.Headers[HttpRequestHeader.Authorization] = string.Format("Bearer {0}", _token.AccessToken);
                    return DownloadStudents(endpoint, client);;
                }
            }
            return null;
        }

        private static ResponseData<ChildInfo> DownloadStudents(string endpoint, System.Net.WebClient client)
        {
            // invoke the REST method
            var jsonData = client.DownloadString(endpoint);

            var responseData = JsonConvert.DeserializeObject<ResponseData<ChildInfo>>(jsonData);
            return responseData;
        }
    }


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
