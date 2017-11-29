using System;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Json.Jil;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Esynctraining.AspNetCore.Formatters
{
    // https://www.codefluff.com/write-your-own-asp-net-core-mvc-formatters/
    public class JilOutputFormatter : IOutputFormatter
    {
        private static readonly string ContentType = "application/json";


        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.ContentType == null || context.ContentType.ToString() == ContentType;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var response = context.HttpContext.Response;
            response.ContentType = ContentType;

            using (var writer = context.WriterFactory(response.Body, Encoding.UTF8))
            {
                Jil.JSON.Serialize(context.Object, writer, JilSerializer.JilOptions);
                await writer.FlushAsync();
            }
        }

    }

}
