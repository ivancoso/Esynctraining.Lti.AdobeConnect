using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class PrincipalDto
    {
        [DataMember]
        public string login { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string principal_id { get; set; }


        public static PrincipalDto Build(Principal arg)
        {
            return new PrincipalDto
            {
                login = arg.Login,
                email = arg.Email,
                name = arg.Name,
                principal_id = arg.PrincipalId,
            };
        }

    }

}
