﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotAmf.ServiceModel {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DotAmf.ServiceModel.Errors", typeof(Errors).Assembly);
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
        ///   Looks up a localized string similar to Operation &apos;{0}&apos; is not supported..
        /// </summary>
        internal static string AmfCommandInvoker_ProcessCommand_OperationNotSupported {
            get {
                return ResourceManager.GetString("AmfCommandInvoker_ProcessCommand_OperationNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AMF message contains no message bodies..
        /// </summary>
        internal static string AmfEncoder_WriteMessage_EmptyAmfMessage {
            get {
                return ResourceManager.GetString("AmfEncoder_WriteMessage_EmptyAmfMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid message type..
        /// </summary>
        internal static string AmfEncoder_WriteMessage_InvalidMessageType {
            get {
                return ResourceManager.GetString("AmfEncoder_WriteMessage_InvalidMessageType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation &apos;{0}&apos; not found..
        /// </summary>
        internal static string AmfFaultInvoker_Invoke_OperationNotFound {
            get {
                return ResourceManager.GetString("AmfFaultInvoker_Invoke_OperationNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument count mismatch..
        /// </summary>
        internal static string AmfGenericOperationFormatter_DeserializeRequest_ArgumentCountMismatch {
            get {
                return ResourceManager.GetString("AmfGenericOperationFormatter_DeserializeRequest_ArgumentCountMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid AMF operation..
        /// </summary>
        internal static string AmfGenericOperationFormatter_DeserializeRequest_InvalidOperation {
            get {
                return ResourceManager.GetString("AmfGenericOperationFormatter_DeserializeRequest_InvalidOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid AMF operation..
        /// </summary>
        internal static string AmfGenericOperationFormatter_SerializeReply_InvalidOperation {
            get {
                return ResourceManager.GetString("AmfGenericOperationFormatter_SerializeReply_InvalidOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AmfGenericMessage was expected at inputs[0].
        /// </summary>
        internal static string AmfGenericOperationInvoker_Invoke_MessageNotFound {
            get {
                return ResourceManager.GetString("AmfGenericOperationInvoker_Invoke_MessageNotFound", resourceCulture);
            }
        }
    }
}
