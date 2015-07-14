using System.Net.Http;
using System.Threading.Tasks;
using PCG.GOAL.ExternalDataService.Interface;

namespace PCG.GOAL.ExternalDataService.Service
{
    public class Content<T> : IContent where T : class
    {
        public HttpContent HttpContent { get; set; }

        public Task<string> ReadAsStringAsync()
        {
            return HttpContent.ReadAsStringAsync();
        }
    }
}