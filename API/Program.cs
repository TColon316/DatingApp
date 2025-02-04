using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add the Application Services from the Extension we created
builder.Services.AddApplicationServices(builder.Configuration);

// Add the Identity Services from the Extension we created
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

/***********************************************************/
/***** All App code below is referred to as Middleware *****/
/***********************************************************/
// Add our custom Exception Handling Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Add the CORS middleware
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));

// Add Authentication middleware (You must Authenticate someone before you can Authorize them so place UseAuthentication above UseAuthorization)
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapControllers();

#region Seeding Data to the Users table - This MUST come BEFORE the app is run (app.Run()) and AFTER MapControllers()
// Create Users in the database upon the first run of the application (assuming no Users already exist in the database)
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

// Need to use a Try/Catch here bc since we're in the Program file, we are not able to use the Exception handling middleware we created for the app
try
{
  var context = services.GetRequiredService<DataContext>();
  await context.Database.MigrateAsync();
  await Seed.SeedUsers(context);
}
catch (Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>();
  logger.LogError(ex, "An error occurred during migration");
}
#endregion

app.Run();
