﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Starcounter.Startup {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Starcounter.Startup.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to There should be exactly one URI argument that is ID of {0} object in DB. If you want to create context manually use {1}.
        /// </summary>
        internal static string ContextMiddleware_ContextIsDbSoUriShouldHaveOneArgument {
            get {
                return ResourceManager.GetString("ContextMiddleware_ContextIsDbSoUriShouldHaveOneArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not create context of type {0} for view-model of type {1}: {2}.
        /// </summary>
        internal static string ContextMiddleware_CouldNotCreateContext {
            get {
                return ResourceManager.GetString("ContextMiddleware_CouldNotCreateContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not resolve dependency of method {0}.
        /// </summary>
        internal static string ContextMiddleware_CouldNotResolveUriToContextDependencies {
            get {
                return ResourceManager.GetString("ContextMiddleware_CouldNotResolveUriToContextDependencies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please mark {0} as IBound to a database type or use {1}.
        /// </summary>
        internal static string ContextMiddleware_MarkViewModelAsIBoundOrUriToContext {
            get {
                return ResourceManager.GetString("ContextMiddleware_MarkViewModelAsIBoundOrUriToContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not create page of type &apos;{0}&apos;.
        /// </summary>
        internal static string DefaultPageCreator_CouldNotCreatePage {
            get {
                return ResourceManager.GetString("DefaultPageCreator_CouldNotCreatePage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not instantiate parameter &apos;{0}&apos;.
        /// </summary>
        internal static string DefaultPageCreator_CouldNotInstantiateParameter {
            get {
                return ResourceManager.GetString("DefaultPageCreator_CouldNotInstantiateParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type implements {0}, but doesn&apos;t declare a public, non-static method &apos;Init&apos;.
        /// </summary>
        internal static string DefaultPageCreator_TypeImplementsInitWithDependenciesBadly {
            get {
                return ResourceManager.GetString("DefaultPageCreator_TypeImplementsInitWithDependenciesBadly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to created master page is null.
        /// </summary>
        internal static string MasterPageMiddleware_MasterPageIsNull {
            get {
                return ResourceManager.GetString("MasterPageMiddleware_MasterPageIsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid application of attribute {0} in type {1}: {2}.
        /// </summary>
        internal static string ReflectionHelper_InvalidApplicationOfAttribute {
            get {
                return ResourceManager.GetString("ReflectionHelper_InvalidApplicationOfAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method marked with {0} should be public. Method {1} is not.
        /// </summary>
        internal static string ReflectionHelper_MethodNotPublic {
            get {
                return ResourceManager.GetString("ReflectionHelper_MethodNotPublic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method marked with {0} should be static. Method {1} is not.
        /// </summary>
        internal static string ReflectionHelper_MethodNotStatic {
            get {
                return ResourceManager.GetString("ReflectionHelper_MethodNotStatic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method marked with {0} should accept first parameter of type {1}. Method {2} does not conform.
        /// </summary>
        internal static string ReflectionHelper_MethodShouldAcceptOneParameter {
            get {
                return ResourceManager.GetString("ReflectionHelper_MethodShouldAcceptOneParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please mark only one public static method with {0}. Marked methods: {1}.
        /// </summary>
        internal static string ReflectionHelper_MultipleMethods {
            get {
                return ResourceManager.GetString("ReflectionHelper_MultipleMethods", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method marked with {0} should return {1} or its subclass. Method {2} does not conform.
        /// </summary>
        internal static string ReflectionHelper_WrongReturnType {
            get {
                return ResourceManager.GetString("ReflectionHelper_WrongReturnType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not supported: more than 4 parameters in URL.
        /// </summary>
        internal static string Router_MoreParametersNotSupported {
            get {
                return ResourceManager.GetString("Router_MoreParametersNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registering URI &apos;{0}&apos; with type &apos;{1}&apos;.
        /// </summary>
        internal static string Router_RegisteringUri {
            get {
                return ResourceManager.GetString("Router_RegisteringUri", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type {0} has no {1} applied.
        /// </summary>
        internal static string Router_TypeHasNoUrlAttribute {
            get {
                return ResourceManager.GetString("Router_TypeHasNoUrlAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to View-model {0} is registered with malformed URI: &apos;{1}&apos;. URI has to start with a slash sign (&apos;/&apos;).
        /// </summary>
        internal static string Router_ViewModelRegisteredWithMalformedUri {
            get {
                return ResourceManager.GetString("Router_ViewModelRegisteredWithMalformedUri", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to View-model {0} is registered with partial URI: &apos;{1}&apos;. Use &apos;{2}&apos; instead.
        /// </summary>
        internal static string Router_ViewModelRegisteredWithPartialUri {
            get {
                return ResourceManager.GetString("Router_ViewModelRegisteredWithPartialUri", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t fill URI template &apos;{0}&apos; with arguments: provided URI template has {1} slots, but {2} argument(s) were provided. Arguments values: {3}.
        /// </summary>
        internal static string UriHelper_CantFillUriTemplateSlotCountMismatch {
            get {
                return ResourceManager.GetString("UriHelper_CantFillUriTemplateSlotCountMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to URI &apos;{0}&apos; is malformed: URI should start with a &apos;/&apos;.
        /// </summary>
        internal static string UriHelper_MalformedUri {
            get {
                return ResourceManager.GetString("UriHelper_MalformedUri", resourceCulture);
            }
        }
    }
}
