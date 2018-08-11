using System;
using Xunit;

namespace FirebaseAuth.Test
{
    public class IntegrationTest
    {
        [Fact]
        public void TestFirebaseAuthenticationHelper()
        {
            string privateKey = @"-----BEGIN PRIVATE KEY--.... Your private key\n";

            string clientEmail = @"your client email";

            var firebaseHelper = new FirebaseAuthenticationHelper(privateKey, clientEmail);

            var token = firebaseHelper.GenerateToken("testuser");
        }
    }
}
