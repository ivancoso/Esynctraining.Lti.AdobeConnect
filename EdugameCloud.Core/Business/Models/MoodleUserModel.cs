﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

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

        public MoodleUser GetOneByUserIdAndUserName(int userId, string userName)
        {
            return
                this.Repository.FindAll<MoodleUser>(
                new DefaultQueryOver<MoodleUser, int>().GetQueryOver()
                    .Where(x => x.UserId == userId && x.UserName != null && x.UserName == userName)).FirstOrDefault();
        }

        public MoodleUser GetOneByUserId(int userId)
        {
            return
                this.Repository.FindAll<MoodleUser>(
                new DefaultQueryOver<MoodleUser, int>().GetQueryOver()
                    .Where(x => x.UserId == userId).OrderBy(x => x.DateModified).Desc).FirstOrDefault();
        }

        #endregion

    }
}
