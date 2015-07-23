using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using PCG.GOAL.Common.DataAccess;
using PCG.GOAL.Common.Interface;
using PCG.GOAL.WebService.Controllers;

namespace PCG.GOAL.WebService.Test.Controllers
{
    [TestClass()]
    public class AdminControllerTests
    {
        private IDbService _dbService;
        private ServiceAdminController _adminController;
        private SqlConnection _sqlConnection;
        [TestInitialize]
        public void TestInitialize()
        {
            _sqlConnection =new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\Dev\2015\gvtrunk\gvtrunk\web_root\wsRethink\PCG.GOAL.WebService\App_Data\GoalServiceDb.mdf;Integrated Security=True");
            _dbService = new DbService(new SqlDataAccess(_sqlConnection));
            _adminController = new ServiceAdminController(_dbService);
        }
        [TestMethod()]
        public void GetCredentialsTest()
        {
            var result = _adminController.GetCredentials();
            Assert.IsNotNull(result);
        }

    }
}
