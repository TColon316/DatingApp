using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Return UserName
        public static string GetUsername(this ClaimsPrincipal user)
        {
            // Get the UserName from the ClaimsPrincipal
            var username = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("Cannot get UserName from Token");

            return username;
        }

        // Return UserId
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // Get the UserName from the ClaimsPrincipal
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("Cannot get UserName from Token"));

            return userId;
        }
    }
}