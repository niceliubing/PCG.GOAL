using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PCG.GOAL.Common.Models;
using PCG.GOAL.ExternalDataService.Interface;
using PCG.GOAL.ExternalDataService.Model;
using PCG.GOAL.WebService.Controllers;

namespace PCG.GOAL.WebService.Test.Controllers
{
    [TestClass()]
    public class RethinkControllerTest
    {
        private Mock<IGoalService> _rethinkServiceMock;
        private RethinkController _rethinkController; 

        [TestInitialize]
        public void TestInitialize()
        {
            _rethinkServiceMock = new Mock<IGoalService>();
            _rethinkServiceMock.SetupGet(x => x.ServiceConfig).Returns(new ServiceConfig());
            _rethinkController = new RethinkController(_rethinkServiceMock.Object);
        }
        
        [TestMethod()]
        public void GetTest()
        {
            _rethinkServiceMock.Setup(x => x.GetAllStudentsAsync()).Returns(GetResponse);
            var response = _rethinkController.Get();
            _rethinkServiceMock.Verify(x=>x.GetAllStudentsAsync());
            Assert.IsNotNull(response);
        }

        [TestMethod()]
        public void GetByStateTestNumberTest()
        {
            _rethinkServiceMock.Setup(x => x.GetStudentByStateNumberAsync(It.IsAny<string>())).Returns(GetResponse);
            var response = _rethinkController.GetByStateTestNumber("");
            _rethinkServiceMock.Verify(x => x.GetStudentByStateNumberAsync(""));
            Assert.IsNotNull(response);
        }

        [TestMethod()]
        public void GetByIdentityTest()
        {
            _rethinkServiceMock.Setup(x => x.GetStudentByIdentityAsync(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>())).Returns(GetResponse);
            var response = _rethinkController.GetByIdentity("", "", "");
            _rethinkServiceMock.Verify(x => x.GetStudentByIdentityAsync("","",""));
            Assert.IsNotNull(response);
        }

        private Task<ResponseData<ChildInfo>> GetResponse()
        {
            var response = new Task<ResponseData<ChildInfo>>(() => new ResponseData<ChildInfo>());
            return response;
        }
    }
}
