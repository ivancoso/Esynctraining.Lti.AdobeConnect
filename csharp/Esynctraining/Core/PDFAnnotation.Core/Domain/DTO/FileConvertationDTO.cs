using System.Runtime.Serialization;

namespace PDFAnnotation.Core.Domain.DTO
{
    [DataContract]
    public class FileConvertationDTO
    {
        [DataMember]
        public int uploadStatus { get; set; }

        [DataMember]
        public bool physicalFileExist { get; set; }
    }
}
