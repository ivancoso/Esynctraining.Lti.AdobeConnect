using System.Web;
using System.Web.Mvc;

namespace EdugameCloud.ACEvents.Web.Models
{
    /// <summary>
    /// Add to FileContentResult some properties for specifying file name without forcing a download and specifying size.
    /// And add a workaround for allowing error cases to still display error page.
    /// </summary>
    public class FileContentResultEx : FileContentResult
    {
        /// <summary>
        /// In case a file name has been supplied, control whether it should be opened inline or downloaded.
        /// </summary>
        /// <remarks>If <c>FileDownloadName</c> is <c>null</c> or empty, this property has no effect (due to current implementation).</remarks>
        public bool Inline { get; set; }

        /// <summary>
        /// Whether file size should be indicated or not.
        /// </summary>
        /// <remarks>If <c>FileDownloadName</c> is <c>null</c> or empty, this property has no effect (due to current implementation).</remarks>
        public bool IncludeSize { get; set; }

        public FileContentResultEx(byte[] fileContents, string contentType) : base(fileContents, contentType) { }

        public override void ExecuteResult(ControllerContext context)
        {
            FileResultUtils.ExecuteResultWithHeadersRestoredOnFailure(context, base.ExecuteResult);
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            if (Inline)
                FileResultUtils.TweakDispositionAsInline(response);
            if (IncludeSize)
                FileResultUtils.TweakDispositionSize(response, FileContents.LongLength);
            base.WriteFile(response);
        }
    }
}