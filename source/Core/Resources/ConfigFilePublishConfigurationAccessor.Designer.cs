﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
    internal class ConfigFilePublishConfigurationAccessor {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ConfigFilePublishConfigurationAccessor() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuDeploy.Core.Resources.ConfigFilePublishConfigurationAccessor", typeof(ConfigFilePublishConfigurationAccessor).Assembly);
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
        ///   Looks up a localized string similar to The supplied publish configuration could not be saved (Name: {0}, Location: {1}, Api Key: {2})..
        /// </summary>
        internal static string AddOrUpdateErrorFailedMessageTemplate {
            get {
                return ResourceManager.GetString("AddOrUpdateErrorFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publish configuration could not be created (Name: {0}, Location: {1}, Api Key: {2})..
        /// </summary>
        internal static string AddOrUpdateErrorPublishConfigurationCouldNotBeCreatedMessageTemplate {
            get {
                return ResourceManager.GetString("AddOrUpdateErrorPublishConfigurationCouldNotBeCreatedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied publish configuration is not valid (Name: {0}, Location: {1}, Api Key: {2})..
        /// </summary>
        internal static string AddOrUpdateErrorPublishConfigurationIsNotValidMessageTemplate {
            get {
                return ResourceManager.GetString("AddOrUpdateErrorPublishConfigurationIsNotValidMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied publish configuration has been saved successfully (Name: {0}, Location: {1}, Api Key: {2})..
        /// </summary>
        internal static string AddOrUpdateErrorSuccessMessageTemplate {
            get {
                return ResourceManager.GetString("AddOrUpdateErrorSuccessMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publish configuration with the name &quot;{0}&quot; does not exist..
        /// </summary>
        internal static string DeleteConfigurationDoesNotExistMessageTemplate {
            get {
                return ResourceManager.GetString("DeleteConfigurationDoesNotExistMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publish configuration &quot;{0}&quot; could not be deleted..
        /// </summary>
        internal static string DeleteFailureMessageTemplate {
            get {
                return ResourceManager.GetString("DeleteFailureMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publish configuration &quot;{0}&quot; has been successfully deleted..
        /// </summary>
        internal static string DeleteSuccessMessageTemplate {
            get {
                return ResourceManager.GetString("DeleteSuccessMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publish configuration could not be reset..
        /// </summary>
        internal static string ResetFailureMessage {
            get {
                return ResourceManager.GetString("ResetFailureMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publish configuration has been reset successfully..
        /// </summary>
        internal static string ResetSuccessMessage {
            get {
                return ResourceManager.GetString("ResetSuccessMessage", resourceCulture);
            }
        }
    }
}
