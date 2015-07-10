using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.Security;
using PCG.GOAL.ExternalDataService.Interface;

namespace PCG.GOAL.ExternalDataService.Service
{
    public class RethinkGoalService : IGoalService
    {
        public Credentials Credentials { get; set; }
        private readonly IWebServiceClient<ChildInfo> _apiClient;
        private string _baseUrl;
        private string _apiKey;
        private string _getStudentEndpoint;

        private void GetEndpoints()
        {
            _baseUrl = "https://stage-rethinkapi.azurewebsites.net";
            _getStudentEndpoint = "api/children";
            _apiKey = "f6c869277d6b4eaeb9408e90d91ce0a6";

            _baseUrl =ConfigurationManager.AppSettings["Rethink_BaseUrl"];
            _getStudentEndpoint = ConfigurationManager.AppSettings["Rethink_StudentEndpoint"];
            _apiKey = ConfigurationManager.AppSettings["Rethink_ApiKey"];
            if (string.IsNullOrWhiteSpace(_baseUrl) || string.IsNullOrWhiteSpace(_apiKey) ||
                string.IsNullOrWhiteSpace(_getStudentEndpoint))
            {
                throw new Exception("Failed to read Rethink Service Configuration.");
            }
            
        }
        public RethinkGoalService(IWebServiceClient<ChildInfo> apiClient)
        {
            _apiClient = apiClient;
            GetEndpoints();
            _apiClient.BaseUri = _baseUrl;
        }

        public async Task<ResponseData<ChildInfo>> GetAllChildrenAsync()
        {
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}", _getStudentEndpoint, _apiKey);
            var response = await _apiClient.GetAsync();
            return response;
        }

        public IEnumerable<ChildInfo> GetAllChildren()
        {
            return null;
        }

        public async Task<ResponseData<ChildInfo>> GetAllChildByStateNumberAsync(string stateTestNumber)
        {
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}&statetestnumber={2}",
                _getStudentEndpoint, _apiKey, stateTestNumber);
            _apiClient.IsSingleResult = true;
            var response = await _apiClient.GetAsync();
            return response;
        }

        public async Task<ResponseData<ChildInfo>> GetAllChildByIdentityAsync(string firstName, string lastName, string dob)
        {
            //Date of Birth should be passed in YYYYMMDD format
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}&firstname={2}&lastname={3}&dob={4}",
                _getStudentEndpoint, _apiKey, firstName, lastName, dob);
            _apiClient.IsSingleResult = true;
            var response = await _apiClient.GetAsync();
            return response;
        }
    }
}
