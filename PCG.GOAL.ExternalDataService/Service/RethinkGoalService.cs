using System;
using System.Configuration;
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


        public RethinkGoalService(IWebServiceClient<ChildInfo> apiClient, IServiceConfig serviceConfig = null)
        {
            _apiClient = apiClient;
            ServiceConfig = (serviceConfig == null || string.IsNullOrWhiteSpace(serviceConfig.ApiKey)) ? SetServiceConfig() : serviceConfig;
            _apiClient.BaseUri = ServiceConfig.BaseUrl;
        }

        public async Task<ResponseData<ChildInfo>> GetAllStudentsAsync()
        {
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}", ServiceConfig.ServiceEndpoint, ServiceConfig.ApiKey);
            var response = await _apiClient.GetAsync();
            return response;
        }


        public async Task<ResponseData<ChildInfo>> GetStudentByStateNumberAsync(string stateTestNumber)
        {
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}&statetestnumber={2}",
                ServiceConfig.ServiceEndpoint, ServiceConfig.ApiKey, stateTestNumber);
            _apiClient.IsSingleResult = true;
            var response = await _apiClient.GetAsync();
            return response;
        }

        public async Task<ResponseData<ChildInfo>> GetStudentByIdentityAsync(string firstName, string lastName, string dob)
        {
            //Date of Birth should be passed in YYYYMMDD format
            _apiClient.ServiceEndpoint = string.Format("{0}?apikey={1}&firstname={2}&lastname={3}&dob={4}",
                ServiceConfig.ServiceEndpoint, ServiceConfig.ApiKey, firstName, lastName, dob);
            _apiClient.IsSingleResult = true;
            var response = await _apiClient.GetAsync();
            return response;
        }


        private ServiceConfig SetServiceConfig()
        {
            var serviceConfig = new ServiceConfig
            {
                BaseUrl = GetFromServiceConfig("Rethink_BaseUrl"),
                ServiceEndpoint = GetFromServiceConfig("Rethink_StudentEndpoint"),
                ApiKey = GetFromServiceConfig("Rethink_ApiKey")
            };

            return serviceConfig;
        }

        private string GetFromServiceConfig(string key)
        {
            // todo: Rethink service configuration might be read from database instead of from Web.config
            // todo: this depends on requirement
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("Failed to read Rethink Service Configuration.");
            }
            return value;
        }
    }
}
