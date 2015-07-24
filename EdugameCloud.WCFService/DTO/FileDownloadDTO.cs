using System;
using System.Runtime.Serialization;

namespace EdugameCloud.WCFService.DTO
{
    [DataContract]
    public sealed class FileDownloadDTO
    {
        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string fileName { get; set; }

        [DataMember]
        public long sizeInBytes { get; set; }

        [DataMember]
        public DateTime lastModifyDate { get; set; }

        [DataMember]
        public string downloadUrl { get; set; }

    }

}