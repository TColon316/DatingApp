using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Setup DataContext
builder.Services.AddDbContext<DataContext>(options =>
{
  // Tell the app what database technology we're using (in this case Sqlite)
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add CORS to the app
builder.Services.AddCors();

var app = builder.Build();

/***********************************************************/
/***** All App code below is referred to as Middleware *****/
/***********************************************************/
// Add the CORS middleware
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();
