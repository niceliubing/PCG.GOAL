using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCG.GOAL.Common.Security;
using PCG.GOAL.Common.WebAccess;
using PCG.GOAL.ExternalDataService.Interface;
using PCG.GOAL.ExternalDataService.Model;

namespace PCG.GOAL.ExternalDataService.Service
{
    public class WebServiceClient<T> : IWebServiceClient<T> where T : class
    {

        #region Properties
        public Func<Credentials> GetCredentials { get; set; }
        public Action<string> LogError { get; set; }
        public Credentials Credentials { get; set; }
        public bool IsSingleResult { get; set; }

        /// <summary>
        /// Example: http://localhost:55435;
        /// </summary>
        /// 
        public string BaseUri { get; set; }
        /// <summary>
        /// Example: "token"
        /// </summary>
        public string TokenEndpoint { get; set; }

        /// <summary>
        /// Example: "api/student","api/student/5","api/student?id=5"
        /// </summary>
        public string ServiceEndpoint { get; set; }


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
       
        #endregion

        #region Public Methods
        public string GetToken()
        {
            if (GetCredentials != null)
            {
                Credentials = GetCredentials();
            }
            try
            {
                var postBody = string.Format("username={0}&password={1}&grant_type=password", Credentials.Username, Credentials.Password);
                var authentication =
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", Credentials.ClientId, Credentials.ClientSecret)));
                HttpContent requestContent = new StringContent(postBody, Encoding.UTF8,
                    "application/x-www-form-urlencoded");

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        authentication);
                    using (HttpResponseMessage response = httpClient.PostAsync(TokenEndpointUri, requestContent).Result)
                    {

                        if (response.IsSuccessStatusCode)
                        {
                            using (HttpContent content = response.Content)
                            {
                                var tokenResponse = content.ReadAsAsync<TokenResponseModel>().Result;
                                var token = tokenResponse.AccessToken;
                                return token;
                            }
                        }
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




        public async Task<ResponseData<T>> ServiceAction(string token = null,
            ServiceActionType serviceActionType = ServiceActionType.Get,
            IEnumerable<T> records = null)
        {

            var postBody = records == null ? string.Empty : JsonConvert.SerializeObject(records);
            HttpContent requestContent = new StringContent(postBody, Encoding.UTF8, "application/json");
            // HttpContent requestContent = new StringContent(postBody, Encoding.UTF8, "application/xml");

            var responseData = new ResponseData<T>
            {
                Done = false,
                Data = null,
                StatusCode = "",
                Message = ""
            };
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response;
                    switch (serviceActionType)
                    {
                        case ServiceActionType.Get:
                            response = await httpClient.GetAsync(ServiceEndpointUri);
                            break;
                        case ServiceActionType.Post:
                            response = await httpClient.PostAsync(ServiceEndpointUri, requestContent);
                            break;
                        case ServiceActionType.Put:
                            response = await httpClient.PutAsync(ServiceEndpointUri, requestContent);
                            break;
                        case ServiceActionType.Delete:
                            response = await httpClient.PostAsync(ServiceEndpointUri, requestContent);
                            break;
                        default: // Get
                            response = await httpClient.GetAsync(ServiceEndpointUri);
                            break;
                    }

                    using (response)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (HttpContent content = response.Content)
                            {
                                if (IsSingleResult)
                                {
                                    var record = content.ReadAsAsync<T>();
                                    if (record != null && record.Result is T)
                                    {
                                        responseData.Done = true;
                                        responseData.Message = "OK";
                                        responseData.Data = new List<T> {record.Result};
                                    }
                                }
                                else
                                {
                                    var record = content.ReadAsAsync<IEnumerable<T>>();
                                    if (record != null && record.Result != null)
                                    {
                                        responseData.Done = true;
                                        responseData.Message = "OK";
                                        responseData.Data = (IList<T>) record.Result;
                                    }
                                }

                            }
                            var message = response.ReasonPhrase == "OK" ? string.Empty : ", " + response.ReasonPhrase;
                            if (responseData.Done != true)
                            {
                                responseData.Message = responseData.Message + message;
                            }
                            return responseData;
                        }

                        var failedEndpoint = response.RequestMessage.ToString().Split('?')[0];
                        responseData = new ResponseData<T>
                        {
                            Done = false,
                            Data = null,
                            StatusCode = response.StatusCode.ToString(),
                            Message = string.Format("{0} -- Failed to get data from {1}",response.ReasonPhrase, failedEndpoint)
                        };
                    }
                }
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

        public T GetLastRecord(string token)
        {
            var actionData = GetAsync(token);

            try
            {
                var records = JsonConvert.DeserializeObject<IEnumerable<T>>(actionData.ToString()).ToList();
                if (!records.Any())
                {
                    return null;
                }
                var lastOne = records.Last();
                return lastOne;

            }
            catch (Exception exception)
            {
                return null;
            }
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
