using System.Collections.Generic;
using System.Threading.Tasks;
using PCG.GOAL.Common.Security;
using PCG.GOAL.Common.WebAccess;

namespace PCG.GOAL.ExternalDataService.Interface
{
    public interface IWebServiceClient<T> where T : class
    {
        Credentials Credentials { get; set; }

        /// <summary>
        /// Example: http://localhost:55435;
        /// </summary>
        /// 
        string BaseUri { get; set; }

        /// <summary>
        /// Example: "token"
        /// </summary>
        string TokenEndpoint { get; set; }

        /// <summary>
        /// Example: "api/student","api/student/5","api/student?id=5"
        /// </summary>
        string ServiceEndpoint { get; set; }
        bool IsSingleResult { get; set; }

        string GetToken();
        Task<ResponseData<T>> GetAsync(string token = null);
        Task<ResponseData<T>> PostAsync(IEnumerable<T> records, string token = null);
        Task<ResponseData<T>> PutAsync(IEnumerable<T> records, string token = null);
        Task<ResponseData<T>> DeleteAsync(IEnumerable<T> records, string token = null);
    }
}