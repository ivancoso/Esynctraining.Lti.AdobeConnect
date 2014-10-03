namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    public class CompanyLms : Entity
    {
        public virtual LmsProvider LmsProvider { get; set; }

        public virtual Company Company { get; set; }

        public virtual string AcServer { get; set; }

        public virtual string AcUsername { get; set; }

        public virtual string AcPassword { get; set; }
        
        public virtual string ConsumerKey { get; set; }

        public virtual string SharedSecret { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual User ModifiedBy { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

    }
}
