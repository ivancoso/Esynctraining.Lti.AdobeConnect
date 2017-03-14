using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    /// <summary>
    /// Add to FileStreamResult some properties for specifying file name without forcing a download and specifying size.
    /// And add a workaround for allowing error cases to still display error page.
    /// </summary>
    public class FileStreamResultEx : FileStreamResult
    {
        /// <summary>
        /// In case a file name has been supplied, control whether it should be opened inline or downloaded.
        /// </summary>
        /// <remarks>If <c>FileDownloadName</c> is <c>null</c> or empty, this property has no effect (due to current implementation).</remarks>
        public bool Inline { get; set; }

        /// <summary>
        /// If greater than <c>0</c>, the content size to include in content-disposition header.
        /// </summary>
        /// <remarks>If <c>FileDownloadName</c> is <c>null</c> or empty, this property has no effect (due to current implementation).</remarks>
        public long Size { get; set; }

        public FileStreamResultEx(Stream fileStream, string contentType) : base(fileStream, contentType) { }

        public override void ExecuteResult(ControllerContext context)
        {
            FileResultUtils.ExecuteResultWithHeadersRestoredOnFailure(context, base.ExecuteResult);
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            if (Inline)
                FileResultUtils.TweakDispositionAsInline(response);
            FileResultUtils.TweakDispositionSize(response, Size);
            base.WriteFile(response);
        }
    }
}