using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class PrincipalDto
    {
        //[DataMember(Name = "login")]
        public string login { get; set; }

        //[DataMember(Name = "email")]
        public string email { get; set; }

        //[DataMember(Name = "name")]
        public string name { get; set; }

        //[DataMember(Name = "principalId")]
        public string principalId { get; set; }


        public static PrincipalDto Build(Principal arg)
        {
            return new PrincipalDto
            {
                login = arg.Login,
                email = arg.Email,
                name = arg.Name,
                principalId = arg.PrincipalId,
            };
        }

    }

}
