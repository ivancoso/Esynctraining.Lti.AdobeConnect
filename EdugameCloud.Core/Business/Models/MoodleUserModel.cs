using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

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

        #endregion

    }
}
