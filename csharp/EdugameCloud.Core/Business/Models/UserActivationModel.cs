namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;

    /// <summary>
    ///     The user model.
    /// </summary>
    public class UserActivationModel : BaseModel<UserActivation, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserActivationModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public UserActivationModel(IRepository<UserActivation, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all paged.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{UserActivation}"/>.
        /// </returns>
        public virtual IFutureValue<UserActivation> GetLatestByUser(int userId)
        {
            var queryOver =
                new DefaultQueryOver<UserActivation, int>().GetQueryOver()
                                                           .Where(x => x.User.Id == userId && x.DateExpires >= DateTime.Now).Take(1);
            return this.Repository.FindOne<UserActivation>(queryOver);
        }

        /// <summary>
        /// The get all by user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{UserActivation}"/>.
        /// </returns>
        public virtual IEnumerable<UserActivation> GetAllByUser(int userId)
        {
            var queryOver = new DefaultQueryOver<UserActivation, int>().GetQueryOver().Where(x => x.User.Id == userId);
            return this.Repository.FindAll<UserActivation>(queryOver);
        }

        /// <summary>
        /// The get one by code.
        /// </summary>
        /// <param name="activationCode">
        /// The activation code.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{UserActivation}"/>.
        /// </returns>
        public IFutureValue<UserActivation> GetOneByCode(string activationCode)
        {
            var queryOver =
                new DefaultQueryOver<UserActivation, int>().GetQueryOver()
                .Where(x => x.ActivationCode == activationCode).Take(1);
            return this.Repository.FindOne<UserActivation>(queryOver);
        }

        /// <summary>
        /// The delete all by user.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public void DeleteAllByUser(User user)
        {
            var activations = this.GetAllByUser(user.Id).ToList();
            if (activations.Any())
            {
                foreach (var avtivation in activations)
                {
                    this.RegisterDelete(avtivation);
                }

                this.Flush();
            }
        }

        #endregion
    }
}