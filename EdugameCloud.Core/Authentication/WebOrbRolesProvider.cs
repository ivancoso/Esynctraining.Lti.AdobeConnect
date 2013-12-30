namespace EdugameCloud.Core.Authentication
{
    using System.Linq;
    using System.ServiceModel;

    using EdugameCloud.Core.Business.Models;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Weborb.Security;

    /// <summary>
    /// The web orb roles provider.
    /// </summary>
    public class WebOrbRolesProvider : IRolesProvider
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get roles.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> array.
        /// </returns>
        public string[] GetRoles()
        {
            return IoC.Resolve<UserRoleModel>().GetAllNames().ToArray();
        }

        /// <summary>
        /// The get user roles.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> array.
        /// </returns>
        public string[] GetUserRoles(string userName)
        {
            var result = new string[] { };
            string currentRole =
                IoC.Resolve<UserModel>().GetOneByEmail(userName).Value.With(x => x.UserRole.UserRoleName);
            if (!string.IsNullOrWhiteSpace(currentRole))
            {
                result = new[] { currentRole };
            }

            return result;
        }

        #endregion
    }
}