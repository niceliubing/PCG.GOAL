using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PCG.GOAL.ExternalDataService.Model;

namespace PCG.GOAL.ExternalDataService.Interface
{
    public interface IWebServiceClient<T> where T : class
    {
        Credentials Credentials { get; set; }

        string BaseUri { get; set; }

        string TokenEndpoint { get; set; }

        string ServiceEndpoint { get; set; }
        Func<Credentials> SetCredentials { get; set; }
        bool IsSingleResult { get; set; }
        IClient HttpClient { get; set; }
        IContent ResponseContent { get; set; }

        string GetToken();
        Task<ResponseData<T>> GetAsync(string token = null);
        Task<ResponseData<T>> PostAsync(IEnumerable<T> records, string token = null);
        Task<ResponseData<T>> PutAsync(IEnumerable<T> records, string token = null);
        Task<ResponseData<T>> DeleteAsync(IEnumerable<T> records, string token = null);
    }
}