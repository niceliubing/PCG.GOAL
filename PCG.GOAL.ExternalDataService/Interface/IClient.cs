using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PCG.GOAL.ExternalDataService.Interface
{
    public interface IClient
    {
        HttpClient HttpClient { get; set; }
        void SetAuthorization(AuthenticationHeaderValue header);
        void SetMediaType(MediaTypeWithQualityHeaderValue mediaType);
        Task<HttpResponseMessage> GetAsync(Uri uri);
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);
        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content);
    }
}
