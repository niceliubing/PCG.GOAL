using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.WebService.Test.WebClient
{
    public class WebClientSampleCode
    {
        #region fields and properties

        private const string TokenType = "Bearer";
        private readonly string _baseUrl;
        private readonly string _endpointToken;
        private readonly string _endpointAllStudents;
        private readonly string _endpointStudentByIdentity;
        private readonly string _endpointStudentByStatetestnumber;
        private readonly Credentials _credentials;
        private Uri TokenUrl {get { return GetUri(_baseUrl, _endpointToken); }}

        #endregion

        #region Constructor

        public WebClientSampleCode(string baseUrl, string endpointToken,
            string endpointAllStudents, string endpointStudentByIdentity, string endpointStudentByStatetestnumber)
        {
            _baseUrl = baseUrl;
            _endpointToken = endpointToken;
            _endpointAllStudents = endpointAllStudents;
            _endpointStudentByIdentity = endpointStudentByIdentity;
            _endpointStudentByStatetestnumber = endpointStudentByStatetestnumber;

            var oauthAccess = new OAuthAccess();
            _credentials = oauthAccess.GetCredentials();
        }

        #endregion

        #region public methods

        public ResponseData<ChildInfo> GetAllStudents(Token token)
        {
            return GetStudentsByEndpoint(GetUri(_baseUrl, _endpointAllStudents),token);
        }
        public ResponseData<ChildInfo> GetStudentByIdentity(Token token)
        {
            return GetStudentsByEndpoint(GetUri(_baseUrl, _endpointStudentByIdentity), token);
        }

        public ResponseData<ChildInfo> GetStudentByStatetestnumber(Token token)
        {
            return GetStudentsByEndpoint(GetUri(_baseUrl, _endpointStudentByStatetestnumber), token);
        }
        public Token GetToken()
        {
            // web client
            var client = new System.Net.WebClient();


            // invoke the REST method
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _credentials.ClientId, _credentials.ClientSecret)));


            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            var postVaules = new NameValueCollection
            {
                {"username",_credentials.Username},
                {"password", _credentials.Password},
                {"grant_type", GrantTpype.Password}
            };
            try
            {
                byte[] result = client.UploadValues(TokenUrl, "POST", postVaules);
                var jsonData = Encoding.UTF8.GetString(result);
                var token = JsonConvert.DeserializeObject<Token>(jsonData);
                return token;
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new WebException("Failed to request access token. Check you OAuth credentials.");
                }
            }
            return null;
        }
        public Token RefreshToken(Token token = null)
        {
            if (token == null)
            {
                token = GetToken();
            }
            if (token == null || string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                return null;
            }
            // web client
            var client = new System.Net.WebClient();


            // add headers
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _credentials.ClientId, _credentials.ClientSecret)));
            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

            // refresh token request doesn't need "username" and "password"
            var postVaules = new NameValueCollection
            {
                {"grant_type", GrantTpype.RefreshToken},
                {"refresh_token", token.RefreshToken}
            };
            try
            {
                byte[] result = client.UploadValues(TokenUrl, "POST", postVaules);
                var jsonData = Encoding.UTF8.GetString(result);
                var refreshToken = JsonConvert.DeserializeObject<Token>(jsonData);
                return refreshToken;
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new WebException("Failed to request access token");
                }
            }
            return null;
        }

        #endregion

        #region private methods
        private ResponseData<ChildInfo> GetStudentsByEndpoint(Uri serviceUri, Token token)
        {
            // web client
            var client = new System.Net.WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Headers[HttpRequestHeader.Authorization] = string.Format("{0} {1}", TokenType, token.AccessToken);

            try
            {
                return DownloadStudents(serviceUri, client);
            }
            catch (WebException wex)
            {
                if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // validate token failed, need to refresh token
                    token = RefreshToken(token);
                    client.Headers[HttpRequestHeader.Authorization] = string.Format("{0} {1}", TokenType, token.AccessToken);
                    return DownloadStudents(serviceUri, client); ;
                }
            }
            return null;
        }

        private static ResponseData<ChildInfo> DownloadStudents(Uri serviceUri, System.Net.WebClient client)
        {
            // invoke the REST method
            var jsonData = client.DownloadString(serviceUri);

            var responseData = JsonConvert.DeserializeObject<ResponseData<ChildInfo>>(jsonData);
            return responseData;
        }

        private Uri GetUri(string baseAddress, string endpoint = "")
        {
            try
            {
                var baseUri = new Uri(baseAddress);
                return new Uri(baseUri, endpoint);
            }
            catch (UriFormatException ex)
            {
                throw new Exception(string.Format("Please check the format of BaseUri or Endpoint"), ex);
            }
        }

        #endregion
    }
}
