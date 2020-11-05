namespace AnonymousChat.WebApi.Host
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class EmailAddressDto
    {
        [DataMember]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataMember]
        public string Name { get; set; }

    }

}