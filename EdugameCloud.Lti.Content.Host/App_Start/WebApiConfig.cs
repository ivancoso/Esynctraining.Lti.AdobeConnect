using System.Web.Http;
using Bmbsqd.JilMediaFormatter;
using Jil;

namespace EdugameCloud.Lti.Content.Host
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.EnableCors();

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.JsonFormatter);

            var options = new Options(false, true, false, Jil.DateTimeFormat.MillisecondsSinceUnixEpoch, true,
                UnspecifiedDateTimeKindBehavior.IsUTC,
                SerializationNameFormat.CamelCase);

            var jsonFormatter = new JilMediaTypeFormatter(options);
            //var mp = jsonFormatter.SupportedMediaTypes;
            config.Formatters.Add(jsonFormatter);
        }

    }

}