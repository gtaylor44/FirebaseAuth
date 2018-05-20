using Microsoft.AspNetCore.Http;
using System.Linq;

namespace FirebaseAuth
{
    /// <summary>
    /// Convenience extension methods
    /// </summary>
    public static class HttpContextExtensions
    {
        public static string GetFirebaseUserId(this HttpContext context)
        {
            return context.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        }
    }
}
