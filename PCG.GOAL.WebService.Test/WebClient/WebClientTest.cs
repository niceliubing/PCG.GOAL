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
        [TestMethod]
        public void CanGetAllStudents()
        {
            // web client
            var client = new System.Net.WebClient();
            client.Headers["Content-type"] = "application/json";

            // invoke the REST method
            var jsonData = client.DownloadString("http://localhost:49193/Contacts.svc/GetAll");

            var students = JsonConvert.DeserializeObject<List<ChildInfo>>(jsonData);

            Assert.IsNotNull(students);
        }

        [TestMethod]
        public void CanGetToken()
        {
            // web client
            var client = new System.Net.WebClient();
            
            var values = new NameValueCollection
            {
                {"username", "admin"},
                {"password", "admin"},
                {"grant_type", "password"},
                //{"client_id", "goalview"},
                //{"client_secret", "goalview"},
                {"refresh_token", ""}
            };
            const string clientId = "goalview";
            const string clientSecret = "goalview";
            const string tokenUrl = "http://goalservice.azurewebsites.net/token";

            // invoke the REST method
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}",clientId,clientSecret)));


            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            try
            {

                byte[] result = client.UploadValues(tokenUrl, "POST", values);
                var jsonData = Encoding.UTF8.GetString(result);
                var token = JsonConvert.DeserializeObject<Token>(jsonData);
                Assert.IsNotNull(token);
                Assert.IsInstanceOfType(token, typeof(Token));
            }
            catch (Exception ex)
            {
                Assert.IsNull(ex);
            }

        }
    }
}
