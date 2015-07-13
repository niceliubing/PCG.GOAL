namespace PCG.GOAL.Common.WebAccess
{
    public interface IServiceConfig
    {
        string BaseUrl { get; set; }
        string ServiceEndpoint { get; set; }
        string TokenEndpoint { get; set; }
        string ApiKey { get; set; }
    }

    public class ServiceConfig : IServiceConfig
    {
        public string BaseUrl { get; set; }
        public string ServiceEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string ApiKey { get; set; }
        
    }

}
