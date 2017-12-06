using System;
using System.Web.Mvc;
using Esynctraining.Core.Json;

namespace EdugameCloud.Lti
{
    internal class JsonNetResult : JsonResult
    {
        private static readonly string AppJsonContentType = "application/json";

        private readonly IJsonSerializer _serializer;


        public JsonNetResult(IJsonSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


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
            
            response.Write(_serializer.JsonSerialize(Data));
        }

    }

}