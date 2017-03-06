using System;
using System.Web.Mvc;
using Jil;

namespace EdugameCloud.Lti
{
    public class JsonNetResult : JsonResult
    {
        private static readonly string AppJsonContentType = "application/json";


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
            
            response.Write(JSON.Serialize(Data, JilSerializer.JilOptions));
        }

    }

}