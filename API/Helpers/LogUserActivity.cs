using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        // This will update the Users LastActive date should the User make any kind of Member requests
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Run the action (or method) and then do something AFTER
            var resultsContext = await next();

            // Check if the User is even authenticated. If not then do nothing further and simply return
            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            // Get the Current User's Username
            var userId = resultsContext.HttpContext.User.GetUserId();

            // Get ahold of the IUserReposity interface to access the interface's methods
            var repo = resultsContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            // Attempt to retrieve the User based on the Username provided
            var user = await repo.GetUserByIdAsync(userId);

            // If no User if sound then simply return
            if (user == null) return;

            // If there is a User found then update their LastActive date to the current DateTime
            user.LastActive = DateTime.UtcNow;

            // Save the changes
            await repo.SaveAllAsync();
        }
    }
}