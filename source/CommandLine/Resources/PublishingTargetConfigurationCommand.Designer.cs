﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuDeploy.CommandLine.Resources {
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
    internal class PublishingTargetConfigurationCommand {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PublishingTargetConfigurationCommand() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuDeploy.CommandLine.Resources.PublishingTargetConfigurationCommand", typeof(PublishingTargetConfigurationCommand).Assembly);
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
        ///   Looks up a localized string similar to Add a publishing target to the publishing target configuration (using named arguments)..
        /// </summary>
        internal static string AddCommandExampleDescriptionNamedArguments {
            get {
                return ResourceManager.GetString("AddCommandExampleDescriptionNamedArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add a publishing target to the publishing target configuration (using positional arguments)..
        /// </summary>
        internal static string AddCommandExampleDescriptionPositionalArguments {
            get {
                return ResourceManager.GetString("AddCommandExampleDescriptionPositionalArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &quot;{0}&quot; command requires {1} parameters. The name of the publishing configuration and the url..
        /// </summary>
        internal static string AddCommandInvalidArgumentCountMessageTemplate {
            get {
                return ResourceManager.GetString("AddCommandInvalidArgumentCountMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The API key for the publishing target (only required for remote targets)..
        /// </summary>
        internal static string ArgumentDescriptionApiKey {
            get {
                return ResourceManager.GetString("ArgumentDescriptionApiKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of the publishing configuration (e.g. &quot;Nuget Gallery&quot;).
        /// </summary>
        internal static string ArgumentDescriptionPublishConfigurationName {
            get {
                return ResourceManager.GetString("ArgumentDescriptionPublishConfigurationName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The action to perform ({0})..
        /// </summary>
        internal static string ArgumentDescriptionPublishingTargetActionTemplate {
            get {
                return ResourceManager.GetString("ArgumentDescriptionPublishingTargetActionTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publishing location/url (e.g &quot;https://nuget.org/api/v2/&quot;, &quot;C:\local-nuget-repository&quot;)..
        /// </summary>
        internal static string ArgumentDescriptionPublishLocation {
            get {
                return ResourceManager.GetString("ArgumentDescriptionPublishLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add, remove or reset the publishing configuration..
        /// </summary>
        internal static string CommandDescriptionText {
            get {
                return ResourceManager.GetString("CommandDescriptionText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove the publishing configuration which matches the supplied name (using positional arguments)..
        /// </summary>
        internal static string DeleteCommandExampleDescriptionPositionalArguments {
            get {
                return ResourceManager.GetString("DeleteCommandExampleDescriptionPositionalArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publishing configuration &quot;{0}&quot; could not be removed..
        /// </summary>
        internal static string DeletePublishingConfigurationFailedMessageTemplate {
            get {
                return ResourceManager.GetString("DeletePublishingConfigurationFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must specify the name of the publishing configuration you want to delete from your configuration..
        /// </summary>
        internal static string DeletePublishingConfigurationNoNameSuppliedMessage {
            get {
                return ResourceManager.GetString("DeletePublishingConfigurationNoNameSuppliedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publishing configuration &quot;{0}&quot; has been successfully removed from your configuration..
        /// </summary>
        internal static string DeletePublishingConfigurationSucceededMessageTemplate {
            get {
                return ResourceManager.GetString("DeletePublishingConfigurationSucceededMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must use one of these actions: {0}..
        /// </summary>
        internal static string InvalidActionNameMessageTemplate {
            get {
                return ResourceManager.GetString("InvalidActionNameMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to List all publishing configurations..
        /// </summary>
        internal static string ListCommandExampleDescription {
            get {
                return ResourceManager.GetString("ListCommandExampleDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are currently no publishing targets configured..
        /// </summary>
        internal static string ListPublishingConfigurationsNoConfigsAvailableMessage {
            get {
                return ResourceManager.GetString("ListPublishingConfigurationsNoConfigsAvailableMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name.
        /// </summary>
        internal static string PublishingConfigurationTableHeadlineColumn1 {
            get {
                return ResourceManager.GetString("PublishingConfigurationTableHeadlineColumn1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Url.
        /// </summary>
        internal static string PublishingConfigurationTableHeadlineColumn2 {
            get {
                return ResourceManager.GetString("PublishingConfigurationTableHeadlineColumn2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reset your publishing configuration..
        /// </summary>
        internal static string ResetCommandExampleDescription {
            get {
                return ResourceManager.GetString("ResetCommandExampleDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resetting your publishing configuration failed..
        /// </summary>
        internal static string ResetPublishingConfigurationFailedMessage {
            get {
                return ResourceManager.GetString("ResetPublishingConfigurationFailedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your publishing configuration has been reset successfully..
        /// </summary>
        internal static string ResetPublishingConfigurationSuccessMessage {
            get {
                return ResourceManager.GetString("ResetPublishingConfigurationSuccessMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 699a4dac-a376....
        /// </summary>
        internal static string SampleApiKey {
            get {
                return ResourceManager.GetString("SampleApiKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://nuget.org/api/v2.
        /// </summary>
        internal static string SamplePublishingLocation {
            get {
                return ResourceManager.GetString("SamplePublishingLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nuget Gallery.
        /// </summary>
        internal static string SamplePublishingTargetName {
            get {
                return ResourceManager.GetString("SamplePublishingTargetName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Saving the publishing configuration with the name &quot;{0}&quot; and the location &quot;{1}&quot; failed..
        /// </summary>
        internal static string SavePublishingConfigurationFailedMessageTemplate {
            get {
                return ResourceManager.GetString("SavePublishingConfigurationFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publishing configuration with the name &quot;{0}&quot; and the location &quot;{1}&quot; has been successfully saved..
        /// </summary>
        internal static string SavePublishingConfigurationSucceededMessageTemplate {
            get {
                return ResourceManager.GetString("SavePublishingConfigurationSucceededMessageTemplate", resourceCulture);
            }
        }
    }
}
