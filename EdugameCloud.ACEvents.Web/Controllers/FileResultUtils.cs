using System;
using System.Web;
using System.Web.Mvc;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public static class FileResultUtils
    {
        public static void ExecuteResultWithHeadersRestoredOnFailure(ControllerContext context, Action<ControllerContext> executeResult)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (executeResult == null)
                throw new ArgumentNullException("executeResult");
            var response = context.HttpContext.Response;
            var previousContentType = response.ContentType;
            try
            {
                executeResult(context);
            }
            catch
            {
                if (response.HeadersWritten)
                    throw;
                // Error logic will usually output a content corresponding to original content type. Restore it if response can still be rewritten.
                // (Error logic should ensure headers positionning itself indeed... But this is not the case at least with HandleErrorAttribute.)
                response.ContentType = previousContentType;
                // If a content-disposition header have been set (through DownloadFilename), it must be removed too.
                response.Headers.Remove(ContentDispositionHeader);
                throw;
            }
        }

        private const string ContentDispositionHeader = "Content-Disposition";

        // Unfortunately, the content disposition generation logic is hidden in an Mvc.Net internal class, while not trivial (UTF-8 support).
        // Hacking it after its generation. 
        // Beware, do not try using System.Net.Mime.ContentDisposition instead, it does not conform to the RFC. It does some base64 UTF-8
        // encoding while it should append '*' to parameter name and use RFC 5987 encoding. http://tools.ietf.org/html/rfc6266#section-4.3
        // And http://stackoverflow.com/a/22221217/1178314 comment.
        // To ask for a fix: https://github.com/aspnet/Mvc
        // Other class : System.Net.Http.Headers.ContentDispositionHeaderValue looks better. But requires to detect if the filename needs encoding
        // and if yes, use the 'Star' suffixed property along with setting the sanitized name in non Star property.
        // MVC 6 relies on ASP.NET 5 https://github.com/aspnet/HttpAbstractions which provide a forked version of previous class, with a method
        // for handling that: https://github.com/aspnet/HttpAbstractions/blob/dev/src/Microsoft.Net.Http.Headers/ContentDispositionHeaderValue.cs
        // MVC 6 stil does not give control on FileResult content-disposition header.
        public static void TweakDispositionAsInline(HttpResponseBase response)
        {
            var disposition = response.Headers[ContentDispositionHeader];
            const string downloadModeToken = "attachment;";
            if (string.IsNullOrEmpty(disposition) || !disposition.StartsWith(downloadModeToken, StringComparison.OrdinalIgnoreCase))
                return;

            response.Headers.Remove(ContentDispositionHeader);
            response.Headers.Add(ContentDispositionHeader, "inline;" + disposition.Substring(downloadModeToken.Length));
        }

        public static void TweakDispositionSize(HttpResponseBase response, long size)
        {
            if (size <= 0)
                return;
            var disposition = response.Headers[ContentDispositionHeader];
            const string sizeToken = "size=";
            // Due to current ancestor semantics (no file => inline, file name => download), handling lack of ancestor content-disposition
            // is non trivial. In this case, the content is by default inline, while the Inline property is <c>false</c> by default.
            // This could lead to an unexpected behavior change. So currently not handled.
            if (string.IsNullOrEmpty(disposition) || disposition.Contains(sizeToken))
                return;

            response.Headers.Remove(ContentDispositionHeader);
            response.Headers.Add(ContentDispositionHeader, disposition + "; " + sizeToken + size.ToString());
        }
    }
}