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
    internal class PublishCommand {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PublishCommand() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuDeploy.CommandLine.Resources.PublishCommand", typeof(PublishCommand).Assembly);
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
        ///   Looks up a localized string similar to The path of the NuGet package that shall be published..
        /// </summary>
        internal static string ArgumentDescriptionNugetPackagePath {
            get {
                return ResourceManager.GetString("ArgumentDescriptionNugetPackagePath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of the publish configuration to use..
        /// </summary>
        internal static string ArgumentDescriptionPublishConfigurationName {
            get {
                return ResourceManager.GetString("ArgumentDescriptionPublishConfigurationName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Publish the specified NuGet packge using the supplied publish configuration..
        /// </summary>
        internal static string CommandDescriptionText {
            get {
                return ResourceManager.GetString("CommandDescriptionText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Publish the specified NuGet package &quot;{0}&quot; using the publish configuration &quot;{1}&quot; using positional arguments..
        /// </summary>
        internal static string CommandExampleDescription1 {
            get {
                return ResourceManager.GetString("CommandExampleDescription1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Publish the specified NuGet package &quot;{0}&quot; using the publish configuration &quot;{1}&quot; using named arguments..
        /// </summary>
        internal static string CommandExampleDescription2 {
            get {
                return ResourceManager.GetString("CommandExampleDescription2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No package path specified..
        /// </summary>
        internal static string NoPackagePathSpecifiedMessage {
            get {
                return ResourceManager.GetString("NoPackagePathSpecifiedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No publish configuration name specified..
        /// </summary>
        internal static string NoPublishConfigurationNameSpecifiedMessage {
            get {
                return ResourceManager.GetString("NoPublishConfigurationNameSpecifiedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Publishing the NuGet package &quot;{0}&quot; with the publish configuration &quot;{1}&quot; failed..
        /// </summary>
        internal static string PublishFailedMessageTemplate {
            get {
                return ResourceManager.GetString("PublishFailedMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package &quot;{0}&quot; has been published successfully using the publish configuration &quot;{1}&quot;..
        /// </summary>
        internal static string PublishSucceededMessageTemplate {
            get {
                return ResourceManager.GetString("PublishSucceededMessageTemplate", resourceCulture);
            }
        }
    }
}
