﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TreeDocs.Auth {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TreeDocs.Auth.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logue com outro serviço.
        /// </summary>
        public static string lbl_UseAnotherServiceToRegister {
            get {
                return ResourceManager.GetString("lbl_UseAnotherServiceToRegister", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email.
        /// </summary>
        public static string lblEmail {
            get {
                return ResourceManager.GetString("lblEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nome.
        /// </summary>
        public static string lblName {
            get {
                return ResourceManager.GetString("lblName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Senha.
        /// </summary>
        public static string lblPassword {
            get {
                return ResourceManager.GetString("lblPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Confirmação da Senha.
        /// </summary>
        public static string lblPasswordConfirmation {
            get {
                return ResourceManager.GetString("lblPasswordConfirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Esta conta está bloqueado. Tente novamente mais tarde..
        /// </summary>
        public static string msg_AccountLockedout {
            get {
                return ResourceManager.GetString("msg_AccountLockedout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usuário ou senha inválidos..
        /// </summary>
        public static string val_InvalidCredentials {
            get {
                return ResourceManager.GetString("val_InvalidCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} deve ter entre {2} e {1} caracteres..
        /// </summary>
        public static string val_InvalidField {
            get {
                return ResourceManager.GetString("val_InvalidField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A confirmação da senha não confere.
        /// </summary>
        public static string val_PasswordConfirmationDontMatch {
            get {
                return ResourceManager.GetString("val_PasswordConfirmationDontMatch", resourceCulture);
            }
        }
    }
}
