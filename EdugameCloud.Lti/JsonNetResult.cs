using System;
using System.Web.Mvc;
using Jil;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

namespace EdugameCloud.Lti
{
    public class JsonNetResult : JsonResult
    {
        private static readonly string AppJsonContentType = "application/json";

        private static readonly Options options = new Options(false, true, false, Jil.DateTimeFormat.MillisecondsSinceUnixEpoch, true,
                UnspecifiedDateTimeKindBehavior.IsUTC,
                SerializationNameFormat.CamelCase);
        //private static readonly JsonSerializerSettings jsonSerializerSettings 
        //    = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };


        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
                ? ContentType
                : AppJsonContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            // If you need special handling, you can call another form of SerializeObject below
            //var serializedObject = JsonConvert.SerializeObject(Data, Formatting.None, jsonSerializerSettings);
            
            response.Write(JSON.Serialize(Data, options));
        }

    }

}