using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;

    using NHibernate;

    using PDFAnnotation.Core.Domain.Entities;

    public class PasswordActivationModel : BaseModel<PasswordActivation, int>
    {
        public PasswordActivationModel(IRepository<PasswordActivation, int> repository)
            : base(repository)
        {
        }


        public IFutureValue<PasswordActivation> GetOneByActivationCode(Guid code)
        {
            var query =
                new DefaultQueryOver<PasswordActivation, int>().GetQueryOver()
                                                               .Where(x => x.PasswordActivationCode == code)
                                                               .Fetch(x => x.Contact).Eager
                                                               .Take(1);
            return this.Repository.FindOne(query);
        }

        public IEnumerable<PasswordActivation> GetAllByContact(int id)
        {
            var query = new DefaultQueryOver<PasswordActivation, int>().GetQueryOver().Where(x => x.Contact.Id == id);
            return this.Repository.FindAll(query);
        }

    }

}