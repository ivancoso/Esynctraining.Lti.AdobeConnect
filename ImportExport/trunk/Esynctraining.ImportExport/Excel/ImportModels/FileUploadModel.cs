using System.IO;

namespace Esynctraining.ImportExport.Excel.ImportModels
{
    public class FileUploadModel
    {
        public string FileName { get; set; }

        public Stream FileContent { get; set; }

    }

}
