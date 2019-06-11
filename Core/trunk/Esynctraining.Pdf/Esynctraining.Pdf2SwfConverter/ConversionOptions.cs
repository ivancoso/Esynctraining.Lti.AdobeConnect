using System;
using System.IO;

namespace Esynctraining.Pdf2SwfConverter
{
    public enum ConversionPath
    {
        OnePdf2OneSwf = 0, //generate one swf per pdf
        OnePdf2PerPageSwf, //generate many swf per each pdf page
        Both // generate both one full swf and per page swfs
    }

    public class ConversionOptions
    {
        public bool OverwriteExistingSwfs { get; set; } 

        public ConversionPath HowToConvert { get; set; }

        public string OutputDirectory { get; set; }

        public FileNamingConventions OutputNamingConventions { get; set; } = new FileNamingConventions();

        public class FileNamingConventions
        {
            private string _pagedSwf = "{inputFileName}{pageNumber}.swf";

            public string PagedSwf {
                get => _pagedSwf;
                set
                {
                    if (value != null && value.Contains("{pageNumber}"))
                    {
                        _pagedSwf = value;
                    }

                    throw new ArgumentOutOfRangeException("value", "Paged naming convention should contain {pageNumber} bit");
                }
            }

            public string Swf { get; set; } = "{inputFileName}.swf";

            public string OptimizedPdf { get; set; } = "{inputFileName}.optimized.pdf";

            public string BuildOptimizedPdfFileName(string inputFileName)
            {
                return OptimizedPdf.Replace("{inputFileName}", Path.GetFileNameWithoutExtension(inputFileName));
            }

            public string BuildPagedSwfFileName(string inputFileName)
            {
                

                //todo think of better way to escape that char
                var fixedFileName = Path.GetFileNameWithoutExtension(inputFileName).Replace(Const.FileNamePageNumberPlaceHolder, "_"); 
                
                return PagedSwf.Replace("{inputFileName}", fixedFileName).Replace("{pageNumber}", Const.FileNamePageNumberPlaceHolder);
            }

            public string BuildSwfFileName(string inputFileName)
            {
                return Swf.Replace("{inputFileName}", Path.GetFileNameWithoutExtension(inputFileName));
            }

        }

    }

}
