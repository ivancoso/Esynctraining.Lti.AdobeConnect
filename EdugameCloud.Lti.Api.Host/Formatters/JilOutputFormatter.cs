using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EdugameCloud.Lti.Api.Host.Formatters
{
    // https://www.codefluff.com/write-your-own-asp-net-core-mvc-formatters/
    public class JilOutputFormatter : IOutputFormatter
    {

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.ContentType == null || context.ContentType.ToString() == "application/json")
                return true;

            return false;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var response = context.HttpContext.Response; response.ContentType = "application/json";

            using (var writer = context.WriterFactory(response.Body, Encoding.UTF8))
            {
                Jil.JSON.Serialize(context.Object, writer, JilSerializer.JilOptions);
                await writer.FlushAsync();
            }
        }

    }

}
