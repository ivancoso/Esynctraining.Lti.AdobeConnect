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
        [DataMember]
        public DateTime DateClosed { get; set; }

        [DataMember]
        public DateTime DateCreated { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PrincipalId { get; set; }

        [DataMember]
        public string ScoId { get; set; }

        [DataMember]
        public string Score { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
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
                Type = (int)src.Type,
                Url = src.Url,
                UserName = src.UserName,
            };
        }
        
    }

}
