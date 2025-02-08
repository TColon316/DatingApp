using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            // Get the UserName from the ClaimsPrincipal
            var username = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("Cannot get UserName from Token");

            return username;
        }
    }
}