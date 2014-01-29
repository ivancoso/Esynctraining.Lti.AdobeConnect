// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAuthorize.cs" company="">
//   
// </copyright>
// <summary>
//   The custom authorize.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.MVC.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MVC.Exceptions;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Utils;

    using NHibernate.Mapping;

    /// <summary>
    ///     The custom authorize.
    /// </summary>
    public class CustomAuthorize : AuthorizeAttribute
    {
        #region Constructors and Destructors

        public CustomAuthorize()
        {
            this.RolesList = new List<UserRoleEnum>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthorize"/> class.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        public CustomAuthorize(params UserRoleEnum[] roles)
        {
            this.RolesList = roles.ToList();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the roles list.
        /// </summary>
        public IEnumerable<UserRoleEnum> RolesList { get; set; }

        public bool RedirectToLogin { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The authorize core.
        /// </summary>
        /// <param name="httpContext">
        /// The http context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.User == null)
            {
                return false;
            }

            IPrincipal userContext = httpContext.User;
            if (!userContext.Identity.IsAuthenticated)
            {
                return false;
            }

            var model = IoC.Resolve<AuthenticationModel>();
            var user = (User)model.GetCurrentUser(x => IoC.Resolve<UserModel>().GetOneByEmail(x).Value);
            if (user == null)
            {
                return false;
            }

            return this.RolesList == null || !this.RolesList.Any() || this.RolesList.Contains(UserRoleEnum.Any) || user.IsInAnyRole(this.RolesList.ToArray());
        }

        /// <summary>
        /// The handle unauthorized request.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        /// <exception cref="AccessDeniedException">
        /// </exception>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!RedirectToLogin)
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                filterContext.RequestContext.HttpContext.Response.StatusDescription = "Forbidden";
                filterContext.RequestContext.HttpContext.Response.End();
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);    
            }
        }

        /// <summary>
        /// The safe split.
        /// </summary>
        /// <param name="toSplit">
        /// The to split.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] SafeSplit(string toSplit)
        {
            if (string.IsNullOrEmpty(toSplit))
            {
                return new string[0];
            }

            string[] splitString = toSplit.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return splitString;
        }

        #endregion
    }
}