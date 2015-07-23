using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebModels;
using PCG.GOAL.ExternalDataService.Interface;
using PCG.GOAL.ExternalDataService.Service;

namespace PCG.GOAL.WebService.Test.Service
{
    [TestClass()]
    public class RethinkGoalServiceTests
    {
        public Mock<IWebServiceClient<ChildInfo>> ApiClientMock { get; set; }

        private IServiceConfig _serviceConfig;
        public RethinkGoalService GoalService { get; set; }

        [TestInitialize]
        public void RethinkGoalServiceTest()
        {
            _serviceConfig= new ServiceConfig{ApiKey = "key",BaseUrl = "http://base.com",ServiceEndpoint = "service",TokenEndpoint = "token"};
            ApiClientMock= new Mock<IWebServiceClient<ChildInfo>>();
            ApiClientMock.Setup(x => x.GetAsync(null)).Returns(GetResponse);
            GoalService = new RethinkGoalService(ApiClientMock.Object, _serviceConfig);
            GoalService.ServiceConfig=new ServiceConfig();
        }

        [TestMethod()]
        public void GetAllChildrenAsyncTest()
        {
            // Act
            var responsData = GoalService.GetAllStudentsAsync();

            // Assert
            ApiClientMock.Verify(x=>x.GetAsync(null));
            Assert.IsNotNull(responsData);
        }

 
        [TestMethod()]
        public void GetAllChildByStateNumberAsyncTest()
        {
            // Act
            var responsData = GoalService.GetStudentByStateNumberAsync(stateTestNumber:"");

            // Assert
            ApiClientMock.Verify(x => x.GetAsync(null));
            Assert.IsNotNull(responsData);
        }

        [TestMethod()]
        public void GetAllChildByIdentityAsyncTest()
        {
            // Act
            var responsData = GoalService.GetStudentByIdentityAsync(firstName:"",lastName:"",dob:"");

            // Assert
            ApiClientMock.Verify(x => x.GetAsync(null));
            Assert.IsNotNull(responsData);
        }

        private Task<ResponseData<ChildInfo>> GetResponse()
        {
            var response = new Task<ResponseData<ChildInfo>>(()=>new ResponseData<ChildInfo>());
            return response;
        }
    }
}
