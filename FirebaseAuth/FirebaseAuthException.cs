using System;
using System.Collections.Generic;
using System.Text;

namespace FirebaseAuth
{
    public class FirebaseAuthException : Exception
    {
        public FirebaseAuthException(string message) : base(message) { }
    }
}
