﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuDeploy.Core.Resources {
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
    internal class PackageConfigurationAccessor {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PackageConfigurationAccessor() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuDeploy.Core.Resources.PackageConfigurationAccessor", typeof(PackageConfigurationAccessor).Assembly);
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
        ///   Looks up a localized string similar to Cannot add or updat the supplied package info because it is invalid. {0}.
        /// </summary>
        internal static string AddOrUpdateInvalidPackageMessageTemplate {
            get {
                return ResourceManager.GetString("AddOrUpdateInvalidPackageMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Canot save the the the supplied package info ({0})..
        /// </summary>
        internal static string AddOrUpdateSaveFailedMessageTemplate {
            get {
                return ResourceManager.GetString("AddOrUpdateSaveFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package info ({0}) has been saved successfully..
        /// </summary>
        internal static string AddOrUpdateSucceededMessageTemplate {
            get {
                return ResourceManager.GetString("AddOrUpdateSucceededMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package &quot;{0}&quot; could not be removed..
        /// </summary>
        internal static string RemoveFailedMessageTemplate {
            get {
                return ResourceManager.GetString("RemoveFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot remove the package with the id &quot;{0}&quot;, because it does not exist..
        /// </summary>
        internal static string RemovePackageIdNotFoundMessageTemplate {
            get {
                return ResourceManager.GetString("RemovePackageIdNotFoundMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package &quot;{0}&quot; has been successfully removed..
        /// </summary>
        internal static string RemoveSucceededMessageTemplate {
            get {
                return ResourceManager.GetString("RemoveSucceededMessageTemplate", resourceCulture);
            }
        }
    }
}
