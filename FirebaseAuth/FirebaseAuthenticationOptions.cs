using Microsoft.AspNetCore.Authentication;
using System;

namespace FirebaseAuth
{
    public class FirebaseAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Your project id can be found in Firebase console in project settings. 
        /// See here for more info: https://cloud.google.com/resource-manager/docs/creating-managing-projects?hl=en&visit_id=1-636624075461179293-880540410&rd=1#identifying_projects
        /// </summary>
        public string FirebaseProjectId { get; set; }

        /// <summary>
        /// Set up some logging for any unexpected exceptions so you can troubleshoot just incase something goes wrong. 
        /// </summary>
        public Action<Exception> ExceptionLogger { get; set; }

        /// <summary>
        /// If this is set to true, you will be able to use the firebase.auth().signInWithCustomToken(token) method on client side. Otherwise, if false then token generation is managed by Firebase. 
        /// </summary>
        public bool SignInWithCustomTokenMode { get; set; }

        /// <summary>
        /// Client email is required if SignInWithCustomTokenMode is set to TRUE. See docs for instructions. 
        /// </summary>
        public string ClientEmail { get; set; }
    }
}
