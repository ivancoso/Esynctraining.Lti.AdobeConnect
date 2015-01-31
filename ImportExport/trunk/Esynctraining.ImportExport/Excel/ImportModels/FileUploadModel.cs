using System.IO;

namespace VaStart.Common.ImportExport.Excel.ImportModels
{
    public class FileUploadModel
    {
        public string FileName { get; set; }

        public Stream FileContent { get; set; }

    }

}
