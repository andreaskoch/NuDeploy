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
    internal class ConfigFileSourceRepositoryProvider {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ConfigFileSourceRepositoryProvider() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuDeploy.Core.Resources.ConfigFileSourceRepositoryProvider", typeof(ConfigFileSourceRepositoryProvider).Assembly);
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
        ///   Looks up a localized string similar to Could not delete the repository with the supplied name &quot;{0}&quot;..
        /// </summary>
        internal static string DeleteFailedMessageTemplate {
            get {
                return ResourceManager.GetString("DeleteFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The repository with the name &quot;{0}&quot; could not be removed because it does not exist..
        /// </summary>
        internal static string DeleteFailedRepositoryDoesNotExistMessageTemplate {
            get {
                return ResourceManager.GetString("DeleteFailedRepositoryDoesNotExistMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The repository &quot;{0}&quot; has been successfully removed..
        /// </summary>
        internal static string DeleteSucceededMessageTemplate {
            get {
                return ResourceManager.GetString("DeleteSucceededMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resetting the source repository configuration failed..
        /// </summary>
        internal static string ResetFailed {
            get {
                return ResourceManager.GetString("ResetFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resetting the source repository configuration succeeded..
        /// </summary>
        internal static string ResetSucceeded {
            get {
                return ResourceManager.GetString("ResetSucceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The respository could not be saved because it was impossible to create a repository from the supplied parameters (Repository Name: &quot;{0}&quot;, Url: &quot;{1}&quot;)..
        /// </summary>
        internal static string SaveFailedBecauseRepositoryCouldNotBeCreatedMessageTemplate {
            get {
                return ResourceManager.GetString("SaveFailedBecauseRepositoryCouldNotBeCreatedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The repository ({0}) could not be saved..
        /// </summary>
        internal static string SaveFailedMessageTemplate {
            get {
                return ResourceManager.GetString("SaveFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The repository ({0}) has been saved successfully..
        /// </summary>
        internal static string SaveSucceededMessageTemplate {
            get {
                return ResourceManager.GetString("SaveSucceededMessageTemplate", resourceCulture);
            }
        }
    }
}