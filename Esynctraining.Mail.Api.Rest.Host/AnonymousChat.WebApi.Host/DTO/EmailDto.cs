namespace AnonymousChat.WebApi.Host
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class EmailDto
    {
        [DataMember]
        [Required]
        public string TemplateToken { get; set; }

        [DataMember]
        [Required]
        // meeting or recording
        public string AdobeConnectUrl { get; set; }

        [DataMember]
        [Required]
        public List<EmailAddressDto> ToEmails { get; set; }

        [DataMember]
        public List<EmailAddressDto> CcEmails { get; set; }

        [DataMember]
        public List<EmailAddressDto> BccEmails { get; set; }

        [DataMember]
        [Required]
        public string Subject { get; set; }

        [DataMember]
        [Required]
        public string BodyHtml { get; set; }

    }

}