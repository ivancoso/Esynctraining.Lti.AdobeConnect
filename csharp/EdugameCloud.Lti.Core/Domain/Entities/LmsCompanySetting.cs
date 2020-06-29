using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Domain.Entities
{
    public class LmsCompanySetting : Entity
    {
        public virtual LmsCompany LmsCompany { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
    }
}