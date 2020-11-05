using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PDFAnnotation.Core.Domain.DTO
{
    [DataContract]
    public class FileUploader
    {
        [DataMember]
        public FileDTO[] fileDTOs { get; set; }
        [DataMember]
        public Stream stream { get; set; }
    }
}
