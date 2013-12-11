namespace PDFAnnotation.Core.Weborb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Threading;

    using Esynctraining.Core.Attributes;
    using Esynctraining.Core.Extensions;

    using global::Weborb.Security;

    using PDFAnnotation.Core.Contracts;

    /// <summary>
    ///     The web orb authorization handler.
    /// </summary>
    public class WebOrbAuthorizationHandler : IAuthorizationHandler
    {
        #region Fields

        /// <summary>
        /// The locker object.
        /// </summary>
        private readonly object lockerObject = new object();

        /// <summary>
        /// The resources access.
        /// </summary>
        private Dictionary<string, List<string>> resourcesAccess = new Dictionary<string, List<string>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The authorize access.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AuthorizeAccess(object resource, ORBSecurity security)
        {
            return true;
            var key = (string)resource;
            lock (this.lockerObject)
            {
                if (this.resourcesAccess.Count == 0)
                {
                    this.InitializeResourceAccess();
                }
            }

            if (this.resourcesAccess != null && this.resourcesAccess.ContainsKey(key))
            {
                List<string> roles = this.resourcesAccess[key];
                IPrincipal principal = Thread.CurrentPrincipal;

                return principal.Identity.IsAuthenticated && (!roles.Any() || roles.Any(principal.IsInRole));
            }

            return true; // Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize resource access.
        /// </summary>
        private void InitializeResourceAccess()
        {
            lock (this.lockerObject)
            {
                const string HusebycloudWcfservice = "HusebyCloud.WCFService";
                this.resourcesAccess = new Dictionary<string, List<string>>();
                Type searchedInterfaceType = typeof(IContactService);
                foreach (Type typeReflected in Assembly.GetAssembly(searchedInterfaceType).GetTypes())
                {
                    if (typeReflected.Namespace == searchedInterfaceType.Namespace)
                    {
                        MethodInfo[] publicMethods = typeReflected.GetMethods();
                        foreach (MethodInfo publicMethod in publicMethods)
                        {
                            if (Attribute.IsDefined(publicMethod, typeof(OperationContractAttribute), true)
                                && Attribute.IsDefined(publicMethod, typeof(WebOrbRestrictedAccessAttribute), true))
                            {
                                var roles = new List<string>();
                                var ca =
                                    (WebOrbRestrictedAccessAttribute)
                                    publicMethod.GetCustomAttributes(typeof(WebOrbRestrictedAccessAttribute), true)
                                                .With(x => x.FirstOrDefault());
                                if (ca != null)
                                {
                                    if (ca.Roles != null && ca.Roles.Any(x => !roles.Contains(x)))
                                    {
                                        roles.AddRange(ca.Roles.Where(x => !roles.Contains(x)));
                                    }
                                }

                                string key = HusebycloudWcfservice + "." + typeReflected.Name.TrimStart('I') + "#"
                                             + publicMethod.Name;
                                if (!this.resourcesAccess.ContainsKey(key))
                                {
                                    this.resourcesAccess.Add(key, roles);
                                }
                                else
                                {
                                    List<string> value = this.resourcesAccess[key];
                                    this.resourcesAccess[key] = value.Concat(roles.Except(value)).ToList();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}