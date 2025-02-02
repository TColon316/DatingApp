using API.Extensions;
using API.Middleware;

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

app.Run();
