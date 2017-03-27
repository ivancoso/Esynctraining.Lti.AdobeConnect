using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EdugameCloud.Lti.Api.Host.Formatters
{
    // https://www.codefluff.com/write-your-own-asp-net-core-mvc-formatters/
    public class JilInputFormatter : IInputFormatter
    {
        private static readonly string ContentType = "application/json";


        public bool CanRead(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            return contentType == null || contentType == ContentType;
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var request = context.HttpContext.Request;
            if (request.ContentLength == 0)
            {
                if (context.ModelType.GetTypeInfo().IsValueType)
                    return InputFormatterResult.SuccessAsync(Activator.CreateInstance(context.ModelType));
                else return InputFormatterResult.SuccessAsync(null);
            }

            var encoding = Encoding.UTF8;//do we need to get this from the request im not sure yet 
            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                var model = Jil.JSON.Deserialize(reader, context.ModelType, JilSerializer.JilOptions);
                return InputFormatterResult.SuccessAsync(model);
            }
        }
    }

}
