using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCG.GOAL.Common.WebModels;
using PCG.GOAL.ExternalDataService.Interface;

namespace PCG.GOAL.ExternalDataService.Service
{
    public class WebServiceClient<T> : IWebServiceClient<T> where T : class
    {

        #region Properties
        public Func<Credentials> SetCredentials { get; set; }
        public Action<string> LogError { get; set; }
        public Credentials Credentials { get; set; }
        public bool IsSingleResult { get; set; }
        public string BaseUri { get; set; }
        public string TokenEndpoint { get; set; }
        public string ServiceEndpoint { get; set; }

        public IClient HttpClient { get; set; }
        public IContent ResponseContent { get; set; }

        private Uri ServiceEndpointUri
        {
            get { return GetUri(EndpointType.ServiceEndpoint, ServiceEndpoint); }
        }
        private Uri TokenEndpointUri
        {
            get { return GetUri(EndpointType.TokenEndpoint, TokenEndpoint); }
        }
        #endregion

        #region Constructor

        public WebServiceClient()
        {
            HttpClient = new Client<T>();
            ResponseContent = new Content<T>();
        }

        public WebServiceClient(IClient client, IContent content)
        {
            HttpClient = client;
            ResponseContent = content;
        }
        #endregion

        #region Public Methods
        public string GetToken()
        {

            try
            {
                var postBody = string.Format("username={0}&password={1}&grant_type={2}", Credentials.Username, Credentials.Password, GrantTpype.Password);
                var authentication = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", Credentials.ClientId, Credentials.ClientSecret)));
                HttpContent requestContent = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpClient.SetAuthorization(new AuthenticationHeaderValue("Basic", authentication));

                var response = HttpClient.PostAsync(TokenEndpointUri, requestContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    using (ResponseContent.HttpContent = response.Content)
                    {
                        var tokenResponse = ResponseContent.ReadAsStringAsync().Result;
                        var token = JsonConvert.DeserializeObject<Token>(tokenResponse);
                        return token.AccessToken;
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                LogErrorMessage(hre.Message);
            }
            catch (TaskCanceledException taskCanceledException)
            {
                LogErrorMessage(taskCanceledException.Message);
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
            }

            LogErrorMessage("Failed to get token, check your credentials!");
            return "";
        }

        public async Task<ResponseData<T>> GetAsync(string token = null)
        {
            return await ServiceAction(token, ServiceActionType.Get);
        }

        public async Task<ResponseData<T>> PostAsync(IEnumerable<T> records, string token = null)
        {
            return await ServiceAction(token, ServiceActionType.Post, records);
        }
        public async Task<ResponseData<T>> PutAsync(IEnumerable<T> records, string token = null)
        {
            return await ServiceAction(token, ServiceActionType.Put, records);
        }
        public async Task<ResponseData<T>> DeleteAsync(IEnumerable<T> records, string token = null)
        {
            return await ServiceAction(token, ServiceActionType.Delete, records);
        }

        #endregion

        #region Sevice Calls


        private async Task<ResponseData<T>> ServiceAction(string token = null, ServiceActionType serviceActionType = ServiceActionType.Get,
            IEnumerable<T> records = null)
        {

            var postBody = records == null ? string.Empty : JsonConvert.SerializeObject(records);
            HttpContent requestContent = new StringContent(postBody, Encoding.UTF8, "application/json");
            var responseData = new ResponseData<T> { Done = false, Data = null, StatusCode = "", Message = "" };
            try
            {

                if (!string.IsNullOrEmpty(token))
                {
                    HttpClient.SetAuthorization(new AuthenticationHeaderValue("Bearer", token));
                }
                HttpClient.SetMediaType(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response;
                switch (serviceActionType)
                {
                    case ServiceActionType.Get:
                        response = await HttpClient.GetAsync(ServiceEndpointUri);
                        break;
                    case ServiceActionType.Post:
                        response = await HttpClient.PostAsync(ServiceEndpointUri, requestContent);
                        break;
                    case ServiceActionType.Put:
                        response = await HttpClient.PutAsync(ServiceEndpointUri, requestContent);
                        break;
                    case ServiceActionType.Delete:
                        response = await HttpClient.PostAsync(ServiceEndpointUri, requestContent);
                        break;
                    default: // Get
                        response = await HttpClient.GetAsync(ServiceEndpointUri);
                        break;
                }

                responseData = GetResponse(response);
            }
            catch (HttpRequestException ex)
            {
                LogErrorMessage(ex.Message);
                throw;
            }
            catch (Exception exception)
            {
                LogErrorMessage(exception.Message);
            }
            return responseData;
        }

        private ResponseData<T> GetResponse(HttpResponseMessage response)
        {
            var failedEndpoint = "service endpoint";
            if (response.RequestMessage != null && response.RequestMessage.ToString().IndexOf("?", StringComparison.Ordinal)>0)
            {
                failedEndpoint=response.RequestMessage.ToString().Split('?')[0];
            }
            
            var responseData = new ResponseData<T>
            {
                Done = false,
                Data = null,
                StatusCode = response.StatusCode.ToString(),
                Message = string.Format("{0} -- Failed to get data from {1}", response.ReasonPhrase, failedEndpoint)
            };
            using (response)
            {
                if (response.IsSuccessStatusCode)
                {
                    using (ResponseContent.HttpContent = response.Content)
                    {
                        var result = ResponseContent.ReadAsStringAsync().Result;
                        if (IsSingleResult)
                        {
                            var record = JsonConvert.DeserializeObject<T>(result);
                            if (record != null)
                            {
                                responseData.Done = true;
                                responseData.Message = "OK";
                                responseData.Data = new List<T> { record };
                            }
                        }
                        else
                        {
                            var record = JsonConvert.DeserializeObject<IEnumerable<T>>(result);
                            if (record != null)
                            {
                                responseData.Done = true;
                                responseData.Message = "OK";
                                responseData.Data = (IList<T>)record;
                            }
                        }
                    }
                    var message = response.ReasonPhrase == "OK" ? string.Empty : ", " + response.ReasonPhrase;
                    if (responseData.Done != true)
                    {
                        responseData.Message = responseData.Message + message;
                    }
                }
            }
            return responseData;
        }

        #endregion

        #region Private Helper Methods
        private void LogErrorMessage(string msg)
        {
            if (LogError == null)
            {
                Console.WriteLine(msg);
            }
            else
            {
                LogError(msg);
            }
        }

        private enum EndpointType
        {
            ServiceEndpoint,
            TokenEndpoint
        }

        private Uri GetUri(EndpointType endpointType, string endpoint)
        {
            try
            {
                var baseUri = new Uri(BaseUri);
                return new Uri(baseUri, endpoint);
            }
            catch (UriFormatException ex)
            {
                throw new Exception(string.Format("Please check the format of BaseUri and {0}", endpointType), ex);
            }
        }
        #endregion
    }
}
