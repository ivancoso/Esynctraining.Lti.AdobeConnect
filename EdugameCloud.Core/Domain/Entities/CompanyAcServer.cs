using System;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.Entities
{
    public class CompanyAcServer : Entity
    {
        public virtual string AcServer { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public CompanyAcServer()
        {

        }




    }
}