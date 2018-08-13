using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FirebaseAuth
{

    public class FirebaseAuthenticationHandler : AuthenticationHandler<FirebaseAuthenticationOptions>
    {
        public FirebaseAuthenticationHandler(IOptionsMonitor<FirebaseAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        private void ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(Options.FirebaseProjectId))
            {
                throw new FirebaseAuthException($"Firebase project id is required.");
            }
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {

                ValidateOptions();

                Request.Headers.TryGetValue("Authorization", out var authHeader);

                if (!authHeader.Any())
                {
                    return AuthenticateResult.NoResult();
                }

                var authHeaderValue = authHeader.First();

                if (string.IsNullOrWhiteSpace(authHeaderValue))
                {
                    return AuthenticateResult.NoResult();
                }

                authHeaderValue = authHeaderValue.Replace("Bearer", string.Empty).TrimStart();

                SecurityKey[] keys = await GetSecurityKeys();

                var parameters = new TokenValidationParameters
                {
                    ValidIssuer = "https://securetoken.google.com/" + Options.FirebaseProjectId,
                    ValidAudience = Options.FirebaseProjectId,
                    IssuerSigningKeys = keys,
                    RequireExpirationTime = true,
                };

                // 3. Use JwtSecurityTokenHandler to validate signature, issuer, audience and lifetime
                var handler = new JwtSecurityTokenHandler();

                SecurityToken token;

                ClaimsPrincipal principal = handler.ValidateToken(authHeaderValue, parameters, out token);

                var jwt = (JwtSecurityToken)token;

                // 4.Validate signature algorithm and other applicable valdiations
                if (jwt.Header.Alg != SecurityAlgorithms.RsaSha256)
                {
                    return AuthenticateResult.Fail("The token is not signed with the expected algorithm.");
                }

                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException)
            {
                return AuthenticateResult.Fail("Authorization token has expired.");
            }
            catch (Exception ex)
            {
                if (Options.ExceptionLogger == null)
                {
                    throw new FirebaseAuthException($"No exception logger has been set. Original exception message: {ex.Message}");
                }

                Options.ExceptionLogger(ex);
                return AuthenticateResult.Fail("Failed to authorize user");
            }
        }


        private async Task<SecurityKey[]> GetSecurityKeys()
        {
            // check if there is a cached copy first.
            var cachedKeys = Cache.Instance.Get<SecurityKey[]>(Constants.CertificateKey);
            if (cachedKeys != null)
            {
                return cachedKeys;
            }

            var client = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com/robot/v1/metadata/")
            };

            HttpResponseMessage response = await client.GetAsync("x509/securetoken@system.gserviceaccount.com");
            response.EnsureSuccessStatusCode();

            var x509Data = await response.Content.ReadAsAsync<Dictionary<string, string>>();

            SecurityKey[] keys = x509Data.Values.Select(CreateSecurityKeyFromPublicKey).ToArray();

            var maxAge = response.Headers.CacheControl.MaxAge;
            var dateOfResponse = response.Headers.Date;

            if (maxAge.HasValue && dateOfResponse.HasValue)
            {
                var dateOffset = dateOfResponse.Value.Add(maxAge.Value);
                Cache.Instance.Set(Constants.CertificateKey, keys, dateOffset);
            }

            return keys;
        }

        private DateTimeOffset GetDateTimeOffset(DateTimeOffset dateOfResponse, DateTimeOffset maxAge)
        {
            return DateTime.MaxValue;
        }

        private static SecurityKey CreateSecurityKeyFromPublicKey(string data)
        {
            return new X509SecurityKey(new X509Certificate2(Encoding.UTF8.GetBytes(data)));
        }
    }
}
