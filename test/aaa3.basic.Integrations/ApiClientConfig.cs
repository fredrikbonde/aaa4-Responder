namespace aaa3.basic.Integrations
{
    public class ApiClientConfig
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int TimeoutMilliseconds { get; set; }
        public int Retries { get; set; }
    }
}
