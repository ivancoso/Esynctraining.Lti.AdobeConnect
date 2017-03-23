using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class PrincipalDto
    {
        [Required]
        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Email { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        [Required]
        [DataMember]
        public string PrincipalId { get; set; }


        public static PrincipalDto Build(Principal arg)
        {
            return new PrincipalDto
            {
                Login = arg.Login,
                Email = arg.Email,
                Name = arg.Name,
                PrincipalId = arg.PrincipalId,
            };
        }

    }

}
