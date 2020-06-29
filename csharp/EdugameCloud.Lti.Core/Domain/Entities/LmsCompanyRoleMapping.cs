using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Domain.Entities
{
    public class LmsCompanyRoleMapping : Entity
    {
        public virtual LmsCompany LmsCompany { get; set; }

        public virtual string LmsRoleName { get; set; }

        public virtual int AcRole { get; set; }

        public virtual bool IsDefaultLmsRole { get; set; }

        public virtual bool IsTeacherRole { get; set; }

    }

}