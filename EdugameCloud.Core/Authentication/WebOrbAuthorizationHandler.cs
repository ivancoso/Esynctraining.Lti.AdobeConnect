namespace EdugameCloud.Core.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceModel;
    using System.Threading;

    using EdugameCloud.Core.Attributes;
    using EdugameCloud.Core.Contracts;

    using Esynctraining.Core.Attributes;
    using Esynctraining.Core.Extensions;

    using Weborb.Security;

    using System.Linq;

    public class WebOrbAuthorizationHandler : IAuthorizationHandler
    {

        private Dictionary<string, List<string>> resourcesAccess = new Dictionary<string, List<string>>();

        public bool AuthorizeAccess(object resource, ORBSecurity security)
        {
            return true;
            var key = (string)resource;
            if (resourcesAccess.Count == 0)
            {
                InitializeResourceAccess();
            }

            if (resourcesAccess != null && resourcesAccess.ContainsKey(key))
            {
                var roles = resourcesAccess[key];
                var principal = Thread.CurrentPrincipal;
                
                return principal.Identity.IsAuthenticated && (!roles.Any() || roles.Any(principal.IsInRole));
            }

            return true; //Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }

        private void InitializeResourceAccess()
        {
            lock (resourcesAccess)
            {
                const string EdugameServiceName = "EdugameCloud.WCFService";
                resourcesAccess = new Dictionary<string, List<string>>();
                var searchedInterfaceType = typeof(ITestService);
                if (searchedInterfaceType != null)
                {
                    foreach (var typeReflected in Assembly.GetAssembly(searchedInterfaceType).GetTypes())
                    {
                        if (typeReflected.Namespace == searchedInterfaceType.Namespace)
                        {
                            var publicMethods = typeReflected.GetMethods();
                            foreach (var publicMethod in publicMethods)
                            {
                                if (Attribute.IsDefined(publicMethod, typeof(OperationContractAttribute), true) && Attribute.IsDefined(publicMethod, typeof(WebOrbRestrictedAccessAttribute), true))
                                {
                                    var roles = new List<string>();
                                    var ca = (WebOrbRestrictedAccessAttribute)publicMethod.GetCustomAttributes(typeof(WebOrbRestrictedAccessAttribute), true).With(x => x.FirstOrDefault());
                                    if (ca != null)
                                    {
                                        if (ca.Roles != null && ca.Roles.Any())
                                        {
                                            roles.AddRange(ca.Roles);
                                        }
                                        if (ca.Roles != null && ca.Roles.Any())
                                        {
                                            roles.AddRange(ca.Roles);
                                        }
                                    }

                                    resourcesAccess.Add(EdugameServiceName + "." + typeReflected.Name.TrimStart('I') + "#" + publicMethod.Name, roles);
                                    
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}