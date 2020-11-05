using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Json.Jil;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Esynctraining.AspNetCore.Formatters
{
    // https://www.codefluff.com/write-your-own-asp-net-core-mvc-formatters/
    // TODO: see https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonInputFormatter.cs#L133
    public class JilInputFormatter : TextInputFormatter
    {
        internal class MediaTypeHeaderValues
        {
            public static readonly MediaTypeHeaderValue ApplicationJson
                = MediaTypeHeaderValue.Parse("application/json").CopyAsReadOnly();

            public static readonly MediaTypeHeaderValue TextJson
                = MediaTypeHeaderValue.Parse("text/json").CopyAsReadOnly();

            public static readonly MediaTypeHeaderValue ApplicationJsonPatch
                = MediaTypeHeaderValue.Parse("application/json-patch+json").CopyAsReadOnly();

            public static readonly MediaTypeHeaderValue ApplicationAnyJsonSyntax
                = MediaTypeHeaderValue.Parse("application/*+json").CopyAsReadOnly();
        }

        public JilInputFormatter()
        {
            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);

            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationAnyJsonSyntax);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            using (var reader = new StreamReader(context.HttpContext.Request.Body, encoding))
            {
                try
                {
                    var model = Jil.JSON.Deserialize(reader, context.ModelType, JilSerializer.JilOptions);
                    return InputFormatterResult.SuccessAsync(model);
                }
                catch (Exception ex)
                {
                    // https://github.com/aspnet/Docs/blob/master/aspnetcore/mvc/advanced/custom-formatters/Sample/Formatters/VcardInputFormatter.cs
                    // TODO:                 context.ModelState.TryAddModelError(context.ModelName, ex.Message);
                    context.ModelState.TryAddModelError(string.Empty, ex.Message);
                    return InputFormatterResult.FailureAsync();
                }
            }
        }
    }

}
