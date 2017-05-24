using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Esynctraining.AspNetCore.Formatters
{
    // https://www.codefluff.com/write-your-own-asp-net-core-mvc-formatters/
    // TODO: see https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonInputFormatter.cs#L133
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

            //var encoding = Encoding.UTF8;//do we need to get this from the request im not sure yet 
            using (var reader = new StreamReader(context.HttpContext.Request.Body))
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
