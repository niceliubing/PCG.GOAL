using System;
using System.Threading.Tasks;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebModels;
using PCG.GOAL.ExternalDataService.Interface;

namespace PCG.GOAL.ExternalDataService.Service
{
    public class RethinkGoalService : IGoalService
    {
        public Credentials Credentials { get; set; }
        private readonly IWebServiceClient<ChildInfo> _apiClient;
        public IServiceConfig ServiceConfig { get; set; }

        
        public RethinkGoalService(IWebServiceClient<ChildInfo> apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ResponseData<ChildInfo>> GetAllStudentsAsync()
        {
            SetUrl();
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}", ServiceConfig.ServiceEndpoint, ServiceConfig.ApiKey);
            var response = await _apiClient.GetAsync();
            return response;
        }


        public async Task<ResponseData<ChildInfo>> GetStudentByStateNumberAsync(string stateTestNumber)
        {
            SetUrl();
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}&statetestnumber={2}",
                ServiceConfig.ServiceEndpoint, ServiceConfig.ApiKey, stateTestNumber);
            _apiClient.IsSingleResult = true;
            var response = await _apiClient.GetAsync();
            return response;
        }

        public async Task<ResponseData<ChildInfo>> GetStudentByIdentityAsync(string firstName, string lastName, string dob)
        {
            SetUrl();
            _apiClient.Credentials = new Credentials { Username = "admin", Password = "admin", ClientId = "goalview", ClientSecret = "goalview" };
            _apiClient.TokenEndpoint = "/token";
            _apiClient.GetToken();
            //Date of Birth should be passed in YYYYMMDD format
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}&firstname={2}&lastname={3}&dob={4}",
                ServiceConfig.ServiceEndpoint, ServiceConfig.ApiKey, firstName, lastName, dob);
            _apiClient.IsSingleResult = true;
            var response = await _apiClient.GetAsync();
            return response;
        }

        private void SetUrl()
        {
            if (ServiceConfig == null)
            {
                throw new Exception("ServiceConfig is not set");
            }
            _apiClient.BaseUri = ServiceConfig.BaseUrl;
        }
    }
}
