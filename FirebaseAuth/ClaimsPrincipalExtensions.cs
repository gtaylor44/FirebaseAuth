using System.Linq;
using System.Security.Claims;

namespace FirebaseAuth
{
    /// <summary>
    /// Convenience extension methods
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public static string GetFirebaseUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        }
    }
}
