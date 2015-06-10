using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    /// <summary>
    /// Adobe Connect TransactionInfo DTO.
    /// </summary>
    [DataContract]
    public class TransactionInfoDto
    {
        [DataMember(Name = "dateClosed")]
        public DateTime DateClosed { get; set; }

        [DataMember(Name = "dateCreated")]
        public DateTime DateCreated { get; set; }

        [DataMember(Name = "login")]
        public string Login { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "principalId")]
        public string PrincipalId { get; set; }

        [DataMember(Name = "scoId")]
        public string ScoId { get; set; }

        [DataMember(Name = "score")]
        public string Score { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "transactionId")]
        public string TransactionId { get; set; }

        [DataMember(Name = "type")]
        public ScoType Type { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }


        public static TransactionInfoDto Build(TransactionInfo src)
        {
            return new TransactionInfoDto 
            {
                DateClosed = src.DateClosed,
                DateCreated = src.DateCreated,
                Login = src.Login,
                Name = src.Name,
                PrincipalId = src.PrincipalId,
                ScoId = src.ScoId,
                Score = src.Score,
                Status = src.Status,
                TransactionId = src.TransactionId,
                Type = src.Type,
                Url = src.Url,
                UserName = src.UserName,
            };
        }
        
    }

}
