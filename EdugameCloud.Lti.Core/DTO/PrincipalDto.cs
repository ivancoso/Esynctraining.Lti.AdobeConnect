using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class PrincipalDto
    {
        [DataMember(Name = "login")]
        public string Login { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "principalId")]
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
