namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    
    [DataContract]
    public sealed class EGCVersionsDTO
    {
        [DataMember]
        public VersionDTO adminVersion { get; set; }
        
        [DataMember]
        public VersionDTO publicVersion { get; set; }

    }

}