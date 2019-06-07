using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esynctraining.Pdf2SwfConverter.Model
{
    public class ConversionStatus
    {
        public ConversionStatus()
        {
        }

        public ConversionStatus(ConversionState state, string message = null)
        {
            State = state;
            Message = message;
        }

        public ConversionState State { get; set; }

        public string Message { get; set; }
        public bool RenderEverythingAsBitmap { get; set; }
        public bool OverwriteSource { get; set; }
    }
}
