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
        const string ServiceEndpoint = "/api/rethink/StudentByIdentity?firstname=Brooklin&lastname=Altis&dob=20080605";
        private const string Url = BaseUrl + ServiceEndpoint;

        const string ClientId = "goalview";
        const string ClientSecret = "goalview";
        private string _token = "";
        

        [TestInitialize]
        public void TestInitilaize()
        {
            _token = GetToken().Access_Token;
        }

        [TestMethod]
        public void CanGetAllStudents()
        {
            // web client
            var client = new System.Net.WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Headers[HttpRequestHeader.Authorization] = string.Format("Bearer {0}", _token);

            try
            {

                // invoke the REST method
                var jsonData = client.DownloadString(Url);

                var responseData = JsonConvert.DeserializeObject<ResponseData<ChildInfo>>(jsonData);

                Assert.IsNotNull(responseData);
                Assert.IsTrue(responseData.Done);
                Assert.IsInstanceOfType(responseData.Data, typeof(IEnumerable<ChildInfo>));

            }
            catch (WebException wex)
            {
                if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // validate token failed, need to refresh token
                    Assert.IsNull(wex);
                }
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

        private Token GetToken()
        {
            // web client
            var client = new System.Net.WebClient();


            // invoke the REST method
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", ClientId, ClientSecret)));


            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            var postVaules = new NameValueCollection
            {
                {"username", "admin"},
                {"password", "admin"},
                {"grant_type", "password"},
                {"refresh_token", ""}
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
        private Token RefreshToken(Token token = null)
        {
            if (token == null)
            {
                token = GetToken();
            }
            if (token == null||string.IsNullOrWhiteSpace(token.Refresh_Token))
            {
                return null;
            }
            // web client
            var client = new System.Net.WebClient();


            // add headers
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", ClientId, ClientSecret)));
            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

            // refresh token request doesn't need "username" and "password"
            var postVaules = new NameValueCollection
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", token.Refresh_Token}
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
    }

}
