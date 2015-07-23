using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.WebService.Test.WebClient
{
    /*
     * This TestClass will run the tests against the web servce published to http://goalservice.azurewebsites.net,
     * which therefor retrieves data from service in https://stage-rethinkapi.azurewebsites.net/api/children 
     * with apikey=f6c869277d6b4eaeb9408e90d91ce0a6.
     * if those services are not available, or just don't want to run this Test Class,
     * please comment out all the test methods below
     */
    [TestClass]
    public class WebClientTest
    {
        const string BaseUrl = "http://goalservice.azurewebsites.net";
        const string TokenEndpoint =  "/token";
        const string EndpointAllStudents = "/api/rethink/Student";
        const string EndpointStudentByIdentity = "/api/rethink/StudentByIdentity?firstname=Brooklin&lastname=Altis&dob=20080605";
        const string EndpointStudentByStatetestnumber = "/api/rethink/StudentByStatetestnumber?statetestnumber=2724167171";

        private Token _token;
        private WebClientSampleCode _webClientSampleCode;

        [TestInitialize]
        public void TestInitilaize()
        {
            _webClientSampleCode=new WebClientSampleCode(BaseUrl,TokenEndpoint,
                EndpointAllStudents,EndpointStudentByIdentity,EndpointStudentByStatetestnumber);
            _token = _webClientSampleCode.GetToken();
        }

        [TestMethod]
        public void CanGetStudentByIdentity()
        {
            return;
            try
            {
                var responseData = _webClientSampleCode.GetStudentByIdentity(_token);

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
            return;
            try
            {
                var responseData = _webClientSampleCode.GetStudentByStatetestnumber(_token);

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
            return;
            try
            {
                var responseData = _webClientSampleCode.GetAllStudents(_token);

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
                var token = _webClientSampleCode.GetToken();
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
                var token = _webClientSampleCode.RefreshToken();
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
