using Jose;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace FirebaseAuth
{
    public class FirebaseAuthenticationHelper
    {
        // private_key from the Service Account JSON file
        private readonly string _firebasePrivateKey;

        // Same for everyone
        private const string FirebasePayloadAud = "https://identitytoolkit.googleapis.com/google.identity.identitytoolkit.v1.IdentityToolkit";

        // client_email from the Service Account JSON file
        private readonly string _firebasePayloadIss;
        private readonly string _firebasePayloadSub;

        // the token 'exp' - max 3600 seconds - see https://firebase.google.com/docs/auth/server/create-custom-tokens
        private int FirebaseTokenExpirySecs { get; set; }

        private static RsaPrivateCrtKeyParameters _rsaParams;
        private static object _rsaParamsLocker = new object();

        public FirebaseAuthenticationHelper(string privateKey, string clientEmail, int tokenExpirySeconds = 3600)
        {
            _firebasePrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            _firebasePayloadIss = clientEmail ?? throw new ArgumentNullException(nameof(clientEmail));
            _firebasePayloadSub = clientEmail;
            FirebaseTokenExpirySecs = tokenExpirySeconds;
        }

        public string GenerateToken(string uid)
        {
            return GenerateToken(uid, null);
        }

        public string GenerateToken(string uid, IEnumerable<Claim> claims)
        {
            // Get the RsaPrivateCrtKeyParameters if we haven't already determined them
            if (_rsaParams == null)
            {
                lock (_rsaParamsLocker)
                {
                    if (_rsaParams == null)
                    {
                        using (var streamWriter = WriteToStreamWithString(_firebasePrivateKey.Replace(@"\n", "\n")))
                        {
                            using (var sr = new StreamReader(streamWriter.BaseStream))
                            {
                                var pr = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                                _rsaParams = (RsaPrivateCrtKeyParameters)pr.ReadObject();
                            }
                        }
                    }
                }
            }

            var payload = new Dictionary<string, object> {
                 {"uid", uid}
                ,{"iat", SecondsSinceEpoch(DateTime.UtcNow)}
                ,{"exp", SecondsSinceEpoch(DateTime.UtcNow.AddSeconds(FirebaseTokenExpirySecs))}
                ,{"aud", FirebasePayloadAud}
                ,{"iss", _firebasePayloadIss}
                ,{"sub", _firebasePayloadSub}
            };

            if (claims != null && claims.Any())
            {
                foreach(var claim in claims)
                {
                    if (!payload.ContainsKey(claim.Type))
                    {
                        payload.Add(claim.Type, claim.Value);
                    }
                }
            }

            return JWT.Encode(payload, Org.BouncyCastle.Security.DotNetUtilities.ToRSA(_rsaParams), JwsAlgorithm.RS256);
        }


        private long SecondsSinceEpoch(DateTime dt)
        {
            TimeSpan t = dt - new DateTime(1970, 1, 1);
            return (long)t.TotalSeconds;
        }

        private StreamWriter WriteToStreamWithString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return writer;
        }
    }
}
