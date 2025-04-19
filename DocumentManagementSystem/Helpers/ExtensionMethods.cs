using System.Globalization;
using System.Security.Claims;

namespace DocumentManagementSystem.Helpers
{
    public static class ExtensionMethods
    {
        public static string ToTitleCase(this string s)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
        }
        public static bool IsUserAllowed(this ClaimsPrincipal claimsPrincipal, string userId)
        
            =>claimsPrincipal?.FindFirstValue(Constants.UserId) == userId;
        

    }
}
