namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    /// <summary>
    /// The moodle user model
    /// </summary>
    public class MoodleUserModel : BaseModel<MoodleUser, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public MoodleUserModel(
            IRepository<MoodleUser, int> repository)
            : base(repository)
        {
        }

        #endregion
        #region Public Methods and Operators

        /// <summary>
        /// The get one by user id and user name.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleUser"/>.
        /// </returns>
        public MoodleUser GetOneByUserIdAndUserName(int userId, string userName)
        {
            return
                this.Repository.FindOne(
                new DefaultQueryOver<MoodleUser, int>().GetQueryOver()
                    .Where(x => x.UserId == userId)
                    .AndRestrictionOn(x => x.UserName).IsNotNull
                    .AndRestrictionOn(x => x.UserName).IsInsensitiveLike(userName, MatchMode.Exact)
                    .Take(1)).Value;
        }

        /// <summary>
        /// The get one by user id and token.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleUser"/>.
        /// </returns>
        public MoodleUser GetOneByUserIdAndToken(int userId, string token)
        {
            return
                this.Repository.FindOne(
                new DefaultQueryOver<MoodleUser, int>().GetQueryOver()
                    .Where(x => x.UserId == userId)
                    .AndRestrictionOn(x => x.Token).IsNotNull
                    .AndRestrictionOn(x => x.Token).IsInsensitiveLike(token, MatchMode.Exact)
                    .Take(1)).Value;
        }

        /// <summary>
        /// The get one by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleUser"/>.
        /// </returns>
        public MoodleUser GetOneByUserId(int userId)
        {
            return
                this.Repository.FindOne(new DefaultQueryOver<MoodleUser, int>().GetQueryOver()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.DateModified).Desc
                .Take(1)).Value;
        }

        #endregion

    }
}
