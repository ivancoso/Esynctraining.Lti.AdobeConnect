namespace Esynctraining.ForwardingProxyMiddleware
{
    /// <summary>
    /// Options to configure host, scheme, and port settings
    /// </summary>
    public class ProxyOptions
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
    }
}
