//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option or rebuild the Visual Studio project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Web.Application.StronglyTypedResourceProxyBuilder", "12.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Emails {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Emails() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.Emails", global::System.Reflection.Assembly.Load("App_GlobalResources"));
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EduGame Cloud Account Activation.
        /// </summary>
        internal static string ActivationSubject {
            get {
                return ResourceManager.GetString("ActivationSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account Password Reset.
        /// </summary>
        internal static string ChangePasswordSubject {
            get {
                return ResourceManager.GetString("ChangePasswordSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occured in EduGame Cloud.
        /// </summary>
        internal static string ErrorSubject {
            get {
                return ResourceManager.GetString("ErrorSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to License upgrade is requested..
        /// </summary>
        internal static string LicenseUpgradeRequested {
            get {
                return ResourceManager.GetString("LicenseUpgradeRequested", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome to EduGame Cloud!.
        /// </summary>
        internal static string TrialSubject {
            get {
                return ResourceManager.GetString("TrialSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome to the Team {0}!.
        /// </summary>
        internal static string UserCreatedSubject {
            get {
                return ResourceManager.GetString("UserCreatedSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: Your account has been removed from the system.
        /// </summary>
        internal static string UserDeletedSubject {
            get {
                return ResourceManager.GetString("UserDeletedSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: Your login name has been changed.
        /// </summary>
        internal static string UserEmailEditedSubject {
            get {
                return ResourceManager.GetString("UserEmailEditedSubject", resourceCulture);
            }
        }
    }
}
