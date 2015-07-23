using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PCG.GOAL.Common.DataAccess;
using PCG.GOAL.Common.Interface;


namespace PCG.GOAL.WebService.Test.DataAccess
{
    [TestClass()]
    public class DbServiceTests
    {
        private Mock<ISqlDataAccess> _sqlDataAccessMock;
        private IDbService _dbService;
        [TestInitialize]
        public void TestInitialize()
        {
            _sqlDataAccessMock = new Mock<ISqlDataAccess>();
            _dbService = new DbService(_sqlDataAccessMock.Object);
        }
        [TestMethod()]
        public void GetAllCredentialsTest()
        {
            _sqlDataAccessMock.Setup(x => x.GetTableBySql(It.IsAny<string>(), It.IsAny<bool>())).Returns(GetCredentialsTable());
            var all = _dbService.GetAllCredentials();
            Assert.IsNotNull(all);
            Assert.AreEqual(2, all.ToList().Count);
        }

        [TestMethod()]
        public void GetCredentialsTest()
        {
            _sqlDataAccessMock.Setup(x => x.GetTableBySql(It.IsAny<string>(), It.IsAny<bool>())).Returns(GetCredentialsTable());
            var result = _dbService.GetCredentials("userAdmin");
            Assert.IsNotNull(result);
            Assert.AreEqual("passAdmin", result.Password);
        }
        [TestMethod()]
        public void GetCredentialsByIdTest()
        {
            _sqlDataAccessMock.Setup(x => x.GetTableBySql(It.IsAny<string>(), It.IsAny<bool>())).Returns(GetCredentialsTable());
            var result = _dbService.GetCredentialsById(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("passAdmin", result.Password);
        }



        [TestMethod()]
        public void GetAllClientInfoTest()
        {
            _sqlDataAccessMock.Setup(x => x.GetTableBySql(It.IsAny<string>(), It.IsAny<bool>())).Returns(GetClientTable);
            var all = _dbService.GetAllClientInfo();
            Assert.IsNotNull(all);
            Assert.AreEqual(2, all.ToList().Count);
        }

        [TestMethod()]
        public void GetClientInfoTest()
        {
            _sqlDataAccessMock.Setup(x => x.GetTableBySql(It.IsAny<string>(), It.IsAny<bool>())).Returns(GetClientTable);
            var result = _dbService.GetClientInfo("client_1");
            Assert.IsNotNull(result);
            Assert.AreEqual("secret_1", result.ClientSecret);
        }

        [TestMethod()]
        public void GetClientInfoByIdTest()
        {
            _sqlDataAccessMock.Setup(x => x.GetTableBySql(It.IsAny<string>(), It.IsAny<bool>())).Returns(GetClientTable);
            var result = _dbService.GetClientInfoById(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("secret_1", result.ClientSecret);
        }
        private static DataTable GetCredentialsTable()
        {
            var table = new DataTable();

            table.Columns.Add("id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("password", typeof(string));
            table.Columns.Add("role", typeof(string));


            table.Rows.Add(1, "userAdmin", "passAdmin", "roleAdmin");
            table.Rows.Add(2, "userFoo", "passFoo", "roleFoo");
            return table;
        }

        private static DataTable GetClientTable()
        {
            var table = new DataTable();

            table.Columns.Add("id", typeof(int));
            table.Columns.Add("clientid", typeof(string));
            table.Columns.Add("clientsecret", typeof(string));
            table.Columns.Add("description", typeof(string));


            table.Rows.Add(1, "client_1", "secret_1", "description1");
            table.Rows.Add(2, "client_2", "secret_2", "description2");
            return table;
        }
    }
}
