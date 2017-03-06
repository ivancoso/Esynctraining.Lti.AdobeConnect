using System.Web.Http;
using Bmbsqd.JilMediaFormatter;

namespace EdugameCloud.Lti.Content.Host
{
    internal static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.EnableCors();

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.JsonFormatter);

            var jsonFormatter = new JilMediaTypeFormatter(JilSerializer.JilOptions);
            //var mp = jsonFormatter.SupportedMediaTypes;
            config.Formatters.Add(jsonFormatter);
        }

    }

}