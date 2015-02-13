namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    /// <summary>
    ///     The user role model.
    /// </summary>
    public class UserRoleModel : BaseModel<UserRole, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public UserRoleModel(IRepository<UserRole, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{UserRole}"/>.
        /// </returns>
        public virtual IEnumerable<UserRole> GetAllByName(string name)
        {
            var queryOver =
                new DefaultQueryOver<UserRole, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.UserRoleName)
                                                     .IsInsensitiveLike(name);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public virtual IEnumerable<string> GetAllNames()
        {
            var queryOver = new DefaultQueryOver<UserRole, int>().GetQueryOver().OrderBy(x => x.UserRoleName).Asc.Select(x => x.UserRoleName);
            return this.Repository.FindAll<string>(queryOver);
        }

        #endregion
    }
}