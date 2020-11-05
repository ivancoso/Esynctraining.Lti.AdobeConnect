using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;
    using PDFAnnotation.Core.Domain.Entities;

    public class ContactTypeModel : BaseModel<ContactType, int>
    {
        public ContactTypeModel(IRepository<ContactType, int> repository)
            : base(repository)
        {
        }


        public virtual IEnumerable<ContactType> GetAllByName(string name)
        {
            var queryOver =
                new DefaultQueryOver<ContactType, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.ContactTypeName)
                                                     .IsInsensitiveLike(name);
            return this.Repository.FindAll(queryOver);
        }

        public virtual IEnumerable<string> GetAllNames()
        {
            var queryOver = new DefaultQueryOver<ContactType, int>().GetQueryOver().OrderBy(x => x.ContactTypeName).Asc.Select(x => x.ContactTypeName);
            return this.Repository.FindAll<string>(queryOver);
        }

    }

}