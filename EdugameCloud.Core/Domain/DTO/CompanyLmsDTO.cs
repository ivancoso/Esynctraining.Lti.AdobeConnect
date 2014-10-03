namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    [DataContract]
    public class CompanyLmsDTO
    {
        public CompanyLmsDTO()
        {
            
        }

        public CompanyLmsDTO(CompanyLms instance)
        {
            this.id = instance.Id;
            this.acPassword = instance.AcPassword;
            this.acServer = instance.AcServer;
            this.acUsername = instance.AcUsername;
            this.companyId = instance.Company.Return(x => x.Id, 0);
            this.consumerKey = instance.ConsumerKey;
            this.createdBy = instance.CreatedBy.Return(x => x.Id, 0);
            this.modifiedBy = instance.ModifiedBy.Return(x => x.Id, 0);
            this.dateCreated = instance.DateCreated;
            this.dateModified = instance.DateModified;
            this.lmsProvider = instance.LmsProvider.Return(x => x.LmsProviderName, "");
            this.sharedSecret = instance.SharedSecret;
        }

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int companyId { get; set; }

        [DataMember]
        public string lmsProvider { get; set; }

        [DataMember]
        public string acServer { get; set; }

        [DataMember]
        public string acUsername { get; set; }

        [DataMember]
        public string acPassword { get; set; }

        [DataMember]
        public string consumerKey { get; set; }

        [DataMember]
        public string sharedSecret { get; set; }

        [DataMember]
        public int createdBy { get; set; }

        [DataMember]
        public int modifiedBy { get; set; }

        [DataMember]
        public DateTime dateCreated { get; set; }

        [DataMember]
        public DateTime dateModified { get; set; }

    }
}
