namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class TemplateDTO
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string name { get; set; }
    }
}