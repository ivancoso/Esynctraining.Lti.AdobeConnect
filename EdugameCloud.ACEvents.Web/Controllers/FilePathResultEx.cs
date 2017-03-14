using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public class FilePathResultEx : FilePathResult
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

        public FilePathResultEx(string fileName, string contentType) : base(fileName, contentType) { }

        public override void ExecuteResult(ControllerContext context)
        {
            FileResultUtils.ExecuteResultWithHeadersRestoredOnFailure(context, base.ExecuteResult);
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            if (Inline)
                FileResultUtils.TweakDispositionAsInline(response);
            // File.Exists is more robust than testing through FileInfo, especially in case of invalid path: it does yield false rather than an exception.
            // We wish not to crash here, in order to let FilePathResult crash in its usual way.
            if (IncludeSize && File.Exists(FileName))
            {
                var fileInfo = new FileInfo(FileName);
                FileResultUtils.TweakDispositionSize(response, fileInfo.Length);
            }
            base.WriteFile(response);
        }
    }
}