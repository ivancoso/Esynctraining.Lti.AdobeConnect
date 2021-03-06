﻿namespace PDFAnnotation.Core.Weborb
{
    using System.Linq;
    using Esynctraining.Core.Utils;
    using global::Weborb.Security;

    using PDFAnnotation.Core.Business.Models;

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
            return IoC.Resolve<ContactTypeModel>().GetAllNames().ToArray();
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
            var result = new[] { "Any" };
            return result;
        }

        #endregion
    }
}