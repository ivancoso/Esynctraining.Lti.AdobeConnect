using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.Entities
{
    public class School : Entity
    {
        #region Public Properties
        
        public virtual string AccountName { get; set; }
        //public string State { get; set; }
        public virtual int StateId { get; set; }

        public virtual State State { get; set; }

        public virtual string SchoolNumber { get; set; }
        public virtual string OnsiteOperator { get; set; }
        public virtual string FirstDirector { get; set; }
        public virtual string MainPhone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string SpeedDialNumber { get; set; }
        public virtual string SchoolEmail { get; set; }
        public virtual string CorporateName { get; set; }
        public virtual string FBCRepresentative { get; set; }
        public virtual string ESSRepresentative { get; set; }
        public virtual string StandardsRepresentative { get; set; }
        public virtual string AdvRepresentative { get; set; }
        public virtual string MktgRepresentative { get; set; }

        #endregion

    }
}