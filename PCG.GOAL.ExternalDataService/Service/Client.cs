using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PCG.GOAL.ExternalDataService.Interface;

namespace PCG.GOAL.ExternalDataService.Service
{
    public class Client<T> :IClient where T : class
    {
        public HttpClient HttpClient { get; set; }
        public HttpRequestHeader DefaultRequestHeaders { get; set; }

        public Client()
        {
            HttpClient=new HttpClient();
        }
        public void SetAuthorization(AuthenticationHeaderValue header)
        {
            HttpClient.DefaultRequestHeaders.Authorization = header;
        }

        public void SetMediaType(MediaTypeWithQualityHeaderValue mediaType)
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(mediaType);
        }

        public Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            return HttpClient.GetAsync(uri);
        }

        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
        {
            return HttpClient.PostAsync(uri, content);
        }

        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
        {
            return HttpClient.PutAsync(uri, content);
        }
    }
}