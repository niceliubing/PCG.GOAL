using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PCG.GOAL.Common.Models;
using PCG.GOAL.ExternalDataService.Interface;
using PCG.GOAL.ExternalDataService.Model;
using PCG.GOAL.ExternalDataService.Service;

namespace PCG.GOAL.WebService.Test.Service
{
    [TestClass()]
    public class WebServiceClientTests
    {
        private Mock<IClient> _httpClientMock;
        private Mock<IContent> _responseContentMock;
        IWebServiceClient<ChildInfo> WebServiceClient { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClientMock = new Mock<IClient>();
            _responseContentMock = new Mock<IContent>();
            WebServiceClient = new WebServiceClient<ChildInfo>(_httpClientMock.Object, _responseContentMock.Object);
            WebServiceClient.Credentials = new Credentials { Username = "admin", Password = "admin", ClientId = "goalview", ClientSecret = "goalview" };
            WebServiceClient.BaseUri = "http://localhost";
            WebServiceClient.TokenEndpoint = "/token";
        }

        [TestMethod()]
        public void GetTokenTest()
        {
            // Arrange
            _responseContentMock.Setup(x => x.ReadAsStringAsync()).Returns(GetToken);
            _httpClientMock.Setup(x => x.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>())).Returns(GetResponse);
            
            // Act
            var token = WebServiceClient.GetToken();
            
            // Assert
            _httpClientMock.Verify(x => x.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()));
            _responseContentMock.Verify(x => x.ReadAsStringAsync());
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod()]
        public void GetAsyncTest()
        {            
            // Arrange
            _responseContentMock.Setup(x => x.ReadAsStringAsync()).Returns(GetStudent);
            _httpClientMock.Setup(x => x.GetAsync(It.IsAny<Uri>())).Returns(GetResponse);

            // Act
            var response = WebServiceClient.GetAsync();

            // Assert
            _httpClientMock.Verify(x => x.GetAsync(It.IsAny<Uri>()));
            _responseContentMock.Verify(x => x.ReadAsStringAsync());
            Assert.IsNotNull(response);
        }

        [TestMethod()]
        public void PostAsyncTest()
        {
            // Arrange
            _responseContentMock.Setup(x => x.ReadAsStringAsync()).Returns(GetStudent);
            _httpClientMock.Setup(x => x.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>())).Returns(GetResponse);

            // Act
            var response = WebServiceClient.PostAsync(new List<ChildInfo>());

            // Assert
            _httpClientMock.Verify(x => x.PostAsync(It.IsAny<Uri>(),It.IsAny<HttpContent>()));
            _responseContentMock.Verify(x => x.ReadAsStringAsync());
            Assert.IsNotNull(response);
        }

        [TestMethod()]
        public void PutAsyncTest()
        {
            // No requirement for Put
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void DeleteAsyncTest()
        {
            // No requirement for Delete
            Assert.IsTrue(true);
        }
        private async Task<HttpResponseMessage> GetResponse()
        {
            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri=new Uri("http://localhost/api/student?endpoint=abc");

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                RequestMessage = requestMessage
            };

            return response;
        }
        private async Task<string> GetToken()
        {
            const string response = "{\"access_token\":\"abcde\",\"token_type\":\"bearer\",\"expires_in\":86399,\"refresh_token\":\"fafb7b2b-d351-4076-ba49-ce24de77c104\"}";

            return response;
        }

        private async Task<string> GetStudent()
        {
            var student = new ChildInfo();
            var result = JsonConvert.SerializeObject(student);
            return result;
        }
    }
}
